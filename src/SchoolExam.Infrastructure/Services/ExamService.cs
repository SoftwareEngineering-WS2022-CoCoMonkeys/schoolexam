using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using SchoolExam.Application.Pdf;
using SchoolExam.Application.QrCode;
using SchoolExam.Application.RandomGenerator;
using SchoolExam.Application.Repository;
using SchoolExam.Application.Services;
using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;
using SchoolExam.Domain.Extensions;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Extensions;
using SchoolExam.Infrastructure.Extensions;
using SchoolExam.Infrastructure.Specifications;

namespace SchoolExam.Infrastructure.Services;

public class ExamService : IExamService
{
    private static string _guidRegex = "[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}";
    private static string _pageQrCodeUri = "http://pageQrCode";

    private readonly ILogger<ExamService> _logger;
    private readonly ISchoolExamRepository _repository;

    private readonly IRandomGenerator _randomGenerator;
    private readonly IQrCodeGenerator _qrCodeGenerator;
    private readonly IPdfService _pdfService;
    private readonly IQrCodeReader _qrCodeReader;

    public ExamService(ILogger<ExamService> logger, ISchoolExamRepository repository,
        IRandomGenerator randomGenerator, IQrCodeGenerator qrCodeGenerator, IPdfService pdfService,
        IQrCodeReader qrCodeReader)
    {
        _logger = logger;
        _repository = repository;
        _randomGenerator = randomGenerator;
        _qrCodeGenerator = qrCodeGenerator;
        _pdfService = pdfService;
        _qrCodeReader = qrCodeReader;
    }

    public Exam? GetById(Guid examId)
    {
        var result = _repository.Find<Exam, Guid>(examId);
        return result;
    }

    public IEnumerable<Exam> GetByTeacher(Guid teacherId)
    {
        var result = _repository.List(new ExamByTeacherSpecification(teacherId));
        return result;
    }

    public IEnumerable<Exam> GetByStudent(Guid studentId)
    {
        throw new NotImplementedException();
    }

    public async Task Create(string title, string description, DateTime date, Guid teacherId, string topic)
    {
        var examId = Guid.NewGuid();
        var exam = new Exam(examId, title, description, date, teacherId, new Topic(topic));

        _repository.Add(exam);
        await _repository.SaveChangesAsync();
    }

    public async Task Update(Guid examId, string title, string description, DateTime date)
    {
        var exam = EnsureExamExists(new EntityByIdSpecification<Exam, Guid>(examId));
        exam.Title = title;
        exam.Description = description;
        exam.Date = date;

        _repository.Update(exam);
        await _repository.SaveChangesAsync();
    }

    public async Task Delete(Guid examId)
    {
        var exam = EnsureExamExists(new EntityByIdSpecification<Exam, Guid>(examId));
        if (exam.State.HasBeenBuilt())
        {
            throw new InvalidOperationException("An exam that already has been built must not be deleted.");
        }

        _repository.Remove(exam);
        await _repository.SaveChangesAsync();
    }

    public async Task SetTaskPdfFile(Guid examId, Guid userId, byte[] content)
    {
        var exam = EnsureExamExists(new ExamWithTaskPdfFileByIdSpecification(examId));

        if (exam.State.HasBeenBuilt())
        {
            throw new InvalidOperationException(
                "The task PDF file of an exam that already has been built cannot be changed.");
        }

        // remove previously existing task PDF file
        if (exam.TaskPdfFile != null)
        {
            _repository.Remove(exam.TaskPdfFile);
        }

        var taskPdfFile =
            new TaskPdfFile(Guid.NewGuid(), $"{examId.ToString()}.pdf", content.LongLength, DateTime.Now, userId,
                content, examId);
        // exam is ready to be built after having a task PDF file
        exam.State = ExamState.BuildReady;
        _repository.Update(exam);
        _repository.Add(taskPdfFile);
        await _repository.SaveChangesAsync();
    }

    public async Task FindTasks(Guid examId, Guid userId, params ExamTaskInfo[] tasks)
    {
        var exam = EnsureExamExists(new ExamWithTaskPdfFileAndTasksByIdSpecification(examId));
        if (exam.TaskPdfFile == null)
        {
            throw new InvalidOperationException("Exam does not have a task PDF.");
        }

        // reset existing exam tasks
        var currentExamTasks = exam.Tasks;
        foreach (var task in currentExamTasks)
        {
            _repository.Remove(task);
        }

        var pdf = exam.TaskPdfFile!.Content;
        var links = _pdfService.GetUriLinkAnnotations(pdf).ToList();
        var tasksDict = tasks.ToDictionary(x => x.Id, x => x);
        var startLinkCandidates = links.Where(x => Regex.IsMatch(x.Uri, $"^task-start-{_guidRegex}$"));
        var endLinkCandidatesDict = links.Where(x => Regex.IsMatch(x.Uri, $"^task-end-{_guidRegex}$"))
            .ToDictionary(x => x.Uri, x => x);

        var matchedLinks = new List<PdfUriLinkAnnotationInfo>();
        var matchedTaskIds = new HashSet<Guid>();
        // iterate through detected start markers
        foreach (var startLink in startLinkCandidates)
        {
            // extract task identifier
            var taskIdString = Regex.Match(startLink.Uri, $"{_guidRegex}$").Value;
            var taskId = Guid.Parse(taskIdString);
            // check if the exam has a task with the extracted identifier
            if (tasksDict.ContainsKey(taskId))
            {
                // check if tasked occurs in multiple markers
                if (matchedTaskIds.Contains(taskId))
                {
                    throw new InvalidOperationException($"Task with id {taskId} was found in PDF more than once.");
                }
                
                // find corresponding end marker
                var taskEndString = $"task-end-{taskId}";
                if (!endLinkCandidatesDict.ContainsKey(taskEndString))
                {
                    throw new InvalidOperationException($"No end marker was found for task with id {taskId}.");
                }

                var endLink = endLinkCandidatesDict[taskEndString];

                matchedTaskIds.Add(taskId);
                var task = tasksDict[taskId];
                // add start and end marker to list such that they can be removed from the PDF file afterwards
                matchedLinks.Add(startLink);
                matchedLinks.Add(endLink);
                var examTask = new ExamTask(taskId, task.Title, task.MaxPoints, 1,
                    new ExamPosition(startLink.Page, startLink.Top), new ExamPosition(endLink.Page, endLink.Bottom));
                _repository.Add(examTask);
                exam.Tasks.Add(examTask);
            }
        }

        if (matchedTaskIds.Count != tasks.Length)
        {
            var unmatchedTask = tasks.First(x => !matchedTaskIds.Contains(x.Id));
            throw new InvalidOperationException($"Task with id {unmatchedTask.Id} could not be found in PDF.");
        }

        var pdfWithoutTaskLinks = _pdfService.RemoveUriLinkAnnotations(pdf, matchedLinks.ToArray());
        _repository.Remove(exam.TaskPdfFile);
        var newTaskPdfFile = new TaskPdfFile(Guid.NewGuid(), $"{examId}.pdf", pdfWithoutTaskLinks.LongLength,
            DateTime.Now, userId, pdfWithoutTaskLinks, examId);
        _repository.Add(newTaskPdfFile);
        _repository.Update(exam);

        await _repository.SaveChangesAsync();
    }

    public async Task<int> Build(Guid examId, Guid userId)
    {
        var exam = EnsureExamExists(new ExamWithTaskPdfFileAndParticipantsById(examId));
        var count = exam.Participants.Count(x => x is ExamStudent) +
                    exam.Participants.OfType<ExamCourse>().Sum(x => x.Course.Students.Count);
        if (count < 1)
        {
            throw new ArgumentException("At least one exam booklet must be built.");
        }

        if (exam.TaskPdfFile == null)
        {
            throw new InvalidOperationException("Exam does not have a task PDF file.");
        }
        
        // clean booklets from previous builds
        if (exam.State.HasBeenBuilt())
        {
            await Clean(examId);
        }

        var content = exam.TaskPdfFile.Content;
        var annotations = _pdfService.GetUriLinkAnnotations(content);
        var qrCodeAnnotations = annotations.Where(x => Regex.IsMatch(x.Uri, _pageQrCodeUri)).ToArray();
        var qrCodeAnnotationsDict = qrCodeAnnotations.ToDictionary(x => x.Page, x => x);
        var contentWithoutAnnotations = _pdfService.RemoveUriLinkAnnotations(content, qrCodeAnnotations);

        var pageCount = _pdfService.GetNumberOfPages(contentWithoutAnnotations);
        // generate exam booklets
        for (int i = 0; i < count; i++)
        {
            var qrCodeData = Enumerable.Range(0, pageCount).Select(_ => _randomGenerator.GenerateHexString(32))
                .ToArray();
            var qrCodes = qrCodeData.Select(x => _qrCodeGenerator.GeneratePngQrCode(x)).ToArray();
            var pdfImageInfos = Enumerable.Range(1, pageCount)
                .Select(x =>
                {
                    // check if a QR code placeholder has been detected
                    var qrCodeAnnotation = qrCodeAnnotationsDict.ContainsKey(x) ? qrCodeAnnotationsDict[x] : null;
                    // add QR code add position of QR code placeholder or use default position otherwise
                    return new PdfImageRenderInfo(x, qrCodeAnnotation?.Left ?? 10.0f, qrCodeAnnotation?.Bottom ?? 10.0f,
                        qrCodeAnnotation?.Width ?? 42.0f, qrCodes[x - 1]);
                }).ToArray();
            var bookletContent = _pdfService.RenderImages(contentWithoutAnnotations, pdfImageInfos);

            var bookletId = Guid.NewGuid();
            var bookletPdfFile = new BookletPdfFile(Guid.NewGuid(), $"{i + 1}.pdf", bookletContent.LongLength,
                DateTime.Now, userId, bookletContent, bookletId);
            var booklet = new Booklet(bookletId, examId, i + 1, bookletPdfFile);

            for (int page = 1; page <= pageCount; page++)
            {
                var bookletPage = new BookletPage(Guid.NewGuid(), page, bookletId, qrCodeData[page - 1]);
                _repository.Add(bookletPage);
            }

            _repository.Add(booklet);
            _repository.Add(bookletPdfFile);
        }

        exam.State = ExamState.SubmissionReady;
        _repository.Update(exam);

        await _repository.SaveChangesAsync();

        return count;
    }

    private async Task Clean(Guid examId)
    {
        var exam = EnsureExamExists(new ExamWithBookletsByIdSpecification(examId));

        var submissionPages = _repository.List(new SubmissionPageByExamSpecification(examId));
        if (submissionPages.Any())
        {
            throw new InvalidOperationException("An exam with existing submission pages must not be cleaned.");
        }

        if (!exam.State.HasBeenBuilt())
        {
            throw new InvalidOperationException("The exam has not been built yet.");
        }

        var booklets = exam.Booklets;
        foreach (var booklet in booklets)
        {
            _repository.Remove(booklet);
        }

        exam.State = ExamState.BuildReady;
        _repository.Update(exam);

        await _repository.SaveChangesAsync();
    }

    public byte[] GetConcatenatedBookletPdfFile(Guid examId)
    {
        var exam = EnsureExamExists(new ExamWithBookletsWithPdfFileByIdSpecification(examId));
        var pdfs = exam.Booklets.OrderBy(x => x.SequenceNumber).Select(x => x.PdfFile.Content).ToArray();
        var result = _pdfService.Merge(pdfs);
        return result;
    }

    public async Task Match(Guid examId, byte[] pdf, Guid userId)
    {
        var exam = EnsureExamExists(
            new ExamWithBookletsWithPagesWithSubmissionPageWithPdfFileByIdSpecification(examId));

        if (exam.State != ExamState.SubmissionReady && exam.State != ExamState.InCorrection &&
            exam.State != ExamState.Corrected)
        {
            throw new InvalidOperationException("Exam is not ready to match submissions.");
        }

        var pages = _pdfService.Split(pdf);

        var submissions = _repository.List<Submission, SubmissionWithPagesSpecification>()
            .ToDictionary(x => x.BookletId, x => x);

        // get participating students
        var examWithStudents = _repository.Find(new ExamWithParticipantsById(examId))!;
        var studentIds = examWithStudents.Participants.OfType<ExamStudent>().Select(x => x.Student)
            .Union(examWithStudents.Participants.OfType<ExamCourse>()
                .SelectMany(x => x.Course.Students.Select(s => s.Student)));
        var students = studentIds.ToDictionary(x => x.QrCode.Data, x => x);

        var updatedSubmissionIds = new List<Guid>();
        for (int page = 1; page <= pages.Count; page++)
        {
            var pagePdf = pages[page - 1];

            // extract QR codes from submission page
            var images = _pdfService.ParseImages(pagePdf).ToList();
            var qrCodes = images.SelectMany(x => _qrCodeReader.ReadQrCodes(x.Data, x.RotationMatrix)).ToList();
            // find student identifier QR code
            var matchedStudentQrCode = qrCodes.SingleOrDefault(qrCode => students.ContainsKey(qrCode.Data))?.Data;
            var studentQrCode = matchedStudentQrCode != null
                ? new SchoolExam.Domain.ValueObjects.QrCode(matchedStudentQrCode)
                : null;
            // find student with identifier QR code
            var student = matchedStudentQrCode != null ? students[matchedStudentQrCode] : null;

            var submissionPageId = Guid.NewGuid();
            var submissionPagePdf = new SubmissionPagePdfFile(Guid.NewGuid(), $"{page}.pdf", pagePdf.LongLength,
                DateTime.Now, userId, pagePdf, submissionPageId);

            // find page identifier QR code
            var bookletPages = exam.Booklets.SelectMany(x => x.Pages);
            // find page with identifier QR code
            var matchedPageQrCode =
                qrCodes.SingleOrDefault(qrCode => bookletPages.Any(x => x.QrCode.Data.Equals(qrCode.Data)));
            if (matchedPageQrCode != null)
            {
                // rotate PDF of submission page according to added QR code on page
                submissionPagePdf.Content = _pdfService.Rotate(submissionPagePdf.Content, -matchedPageQrCode.Degrees);

                var matchedPage = bookletPages.Single(x => x.QrCode.Data.Equals(matchedPageQrCode.Data));

                // get existing submission for booklet
                if (!submissions.ContainsKey(matchedPage.BookletId))
                {
                    // create new submission if there has not been added one yet
                    var newSubmission = new Submission(Guid.NewGuid(), student?.Id, matchedPage.BookletId,
                        DateTime.Now.SetKindUtc());
                    submissions.Add(newSubmission.BookletId, newSubmission);
                    _repository.Add(newSubmission);
                }
                else
                {
                    submissions[matchedPage.BookletId].UpdatedAt = DateTime.Now.SetKindUtc();
                }

                var submission = submissions[matchedPage.BookletId];
                updatedSubmissionIds.Add(submission.Id);

                AssignStudentToSubmission(student?.Id, submission);

                // check if page has already been matched previously
                if (matchedPage.SubmissionPage == null)
                {
                    var submissionPage = new SubmissionPage(submissionPageId, examId, submissionPagePdf, submission.Id,
                        matchedPage.Id, studentQrCode);
                    submission.Pages.Add(submissionPage);
                    _repository.Add(submissionPage);
                }
                else
                {
                    _logger.LogInformation(
                        $"Exam booklet page corresponding to {page} of PDF document has already been matched. The previously matched PDF page was replaced by the most recently parsed page.");

                    var previouslyMatchedPage = submission.Pages.Single(x => x.BookletPageId.Equals(matchedPage.Id));
                    _repository.Remove(previouslyMatchedPage.PdfFile);
                    previouslyMatchedPage.PdfFile = submissionPagePdf;
                    previouslyMatchedPage.StudentQrCode ??= studentQrCode;
                }
            }
            else
            {
                _logger.LogWarning(
                    $"Page {page} of PDF document could not be matched to an existing exam booklet page.");

                // persist unmatched submission pages such that they can be matched manually afterwards
                var submissionPage = new SubmissionPage(submissionPageId, examId, submissionPagePdf, null, null,
                    studentQrCode);
                _repository.Add(submissionPage);
            }

            _repository.Add(submissionPagePdf);
        }

        await _repository.SaveChangesAsync();
        if (updatedSubmissionIds.Count > 0)
        {
            await MergeCompleteSubmissions(examId, updatedSubmissionIds, userId);
            await CheckCompletenessOfExamSubmissions(examId);
        }
    }

    public IEnumerable<SubmissionPage> GetUnmatchedSubmissionPages(Guid examId)
    {
        EnsureExamExists(new EntityByIdSpecification<Exam, Guid>(examId));
        var submissionPages = _repository.List(new SubmissionPageByExamSpecification(examId));

        var result = submissionPages.Where(x => !x.IsMatched);
        return result;
    }

    public IEnumerable<BookletPage> GetUnmatchedBookletPages(Guid examId)
    {
        var exam = EnsureExamExists(new ExamWithBookletsWithPagesWithSubmissionPageByIdSpecification(examId));
        var bookletPages = exam.Booklets.SelectMany(x => x.Pages);

        var result = bookletPages.Where(x => !x.IsMatched);
        return result;
    }

    public async Task MatchManually(Guid examId, Guid bookletPageId, Guid submissionPageId, Guid userId)
    {
        EnsureExamExists(new EntityByIdSpecification<Exam, Guid>(examId));
        var bookletPage = _repository.Find<BookletPage, Guid>(bookletPageId);
        if (bookletPage == null)
        {
            throw new ArgumentException("Booklet page does not exist.");
        }

        var bookletExamId = _repository.Find<Booklet, Guid>(bookletPage.BookletId)!.ExamId;
        if (!examId.Equals(bookletExamId))
        {
            throw new InvalidOperationException("Booklet page is not part of the exam.");
        }

        var submissionPage = _repository.Find<SubmissionPage, Guid>(submissionPageId);
        if (submissionPage == null)
        {
            throw new ArgumentException("Submission page does not exist.");
        }

        if (!examId.Equals(submissionPage.ExamId))
        {
            throw new InvalidOperationException("Submission page is not part of the exam.");
        }

        var bookletPageMatched = _repository.List(new SubmissionPageByBookletPageSpecification(bookletPageId)).Any();

        // not possible to manually match a previously matched pages
        if (bookletPageMatched || submissionPage.BookletPageId.HasValue)
        {
            throw new InvalidOperationException(
                "The booklet page and/or the submission page have already been matched.");
        }

        var bookletId = bookletPage.BookletId;
        var submission = _repository.Find(new SubmissionByBookletSpecification(bookletId)) ??
                         new Submission(Guid.NewGuid(), null, bookletId, DateTime.Now.SetKindUtc());

        var student = submissionPage.StudentQrCode != null
            ? _repository.Find(new StudentByQrCodeSpecification(submissionPage.StudentQrCode.Data))
            : null;
        AssignStudentToSubmission(student?.Id, submission);

        submissionPage.SubmissionId = submission.Id;
        submissionPage.BookletPageId = bookletPageId;
        submission.UpdatedAt = DateTime.Now.SetKindUtc();
        
        _repository.Update(submissionPage);
        await _repository.SaveChangesAsync();

        await MergeCompleteSubmissions(examId, new[] {submission.Id}, userId);
        await CheckCompletenessOfExamSubmissions(examId);
    }

    private Exam EnsureExamExists(EntityByIdSpecification<Exam, Guid> spec)
    {
        var exam = _repository.Find(spec);
        if (exam == null)
        {
            throw new ArgumentException("Exam does not exist.");
        }

        return exam;
    }

    private async Task MergeCompleteSubmissions(Guid examId, IEnumerable<Guid> updatedSubmissionIds, Guid userId)
    {
        var updatedSubmissionIdsSet = updatedSubmissionIds.ToHashSet();
        var updatedSubmissions =
            _repository.List(
                new SubmissionWithAnswersAndPdfFileAndPagesWithPdfFileByIdsSpecification(updatedSubmissionIdsSet));
        var updatedSubmissionsDict = updatedSubmissions.ToDictionary(x => x.BookletId, x => x);
        var exam = _repository.Find(new ExamWithTasksAndBookletsWithPagesWithSubmissionPageByIdSpecification(examId))!;

        // get updated booklets with a complete submission
        var booklets = exam.Booklets
            .Where(x => x.HasCompleteSubmission)
            .Where(x => updatedSubmissionsDict.ContainsKey(x.Id)).ToList();
        var bookletPagesDict = booklets.SelectMany(x => x.Pages).ToDictionary(x => x.Id, x => x.Page);

        // prepare outline to be added to all complete submission PDFs
        var outlineElements = exam.Tasks.Select(x => new PdfOutlineInfo(x.Title, x.Start.Page, (float) x.Start.Y))
            .ToArray();

        // TODO: make sure that nothing has been corrected before deleting anything
        foreach (var booklet in booklets)
        {
            var submission = updatedSubmissionsDict[booklet.Id];
            if (submission.PdfFile != null)
            {
                _repository.Remove(submission.PdfFile);
            }

            // remove previously existing
            foreach (var answer in submission.Answers)
            {
                _repository.Remove(answer);
            }

            // add answers for all tasks to submission
            foreach (var task in exam.Tasks)
            {
                var answer = new Answer(Guid.NewGuid(), task.Id, submission.Id, AnswerState.Pending, null);
                var defaultSegmentAnswer = new AnswerSegment(Guid.NewGuid(),
                    new ExamPosition(task.Start.Page, task.Start.Y), new ExamPosition(task.End.Page, task.End.Y),
                    answer.Id);
                _repository.Add(answer);
                _repository.Add(defaultSegmentAnswer);
            }

            // merge submission pages of booklet ordered page numbers
            var pages = submission.Pages.OrderBy(x => bookletPagesDict[x.BookletPageId!.Value])
                .Select(x => x.PdfFile.Content).ToArray();
            var submissionPdf = _pdfService.Merge(pages);
            var submissionPdfWithOutline = _pdfService.SetTopLevelOutline(submissionPdf, outlineElements);

            var submissionPdfFile = new SubmissionPdfFile(Guid.NewGuid(), $"submission_{submission.Id}.pdf",
                submissionPdfWithOutline.LongLength, DateTime.Now, userId, submissionPdfWithOutline, submission.Id);
            _repository.Add(submissionPdfFile);
        }

        await _repository.SaveChangesAsync();
    }

    private async Task CheckCompletenessOfExamSubmissions(Guid examId)
    {
        var exam = _repository.Find(new ExamWithBookletsWithPagesWithSubmissionPageByIdSpecification(examId))!;
        var hasSubmission = exam.Booklets.Any(x => x.HasCompleteSubmission);
        exam.State = hasSubmission ? ExamState.InCorrection : ExamState.SubmissionReady;

        _repository.Update(exam);
        await _repository.SaveChangesAsync();
    }

    private void AssignStudentToSubmission(Guid? studentId, Submission submission)
    {
        // check if submission has already been assigned to another student
        if (submission.StudentId.HasValue && studentId.HasValue && !studentId.Equals(submission.StudentId))
        {
            throw new InvalidOperationException(
                $"Submission cannot be assigned to student with identifier {studentId} because it was previously assigned to student with identifier {submission.StudentId.Value}");
        }

        // assign student to submission
        submission.StudentId ??= studentId;
    }
}