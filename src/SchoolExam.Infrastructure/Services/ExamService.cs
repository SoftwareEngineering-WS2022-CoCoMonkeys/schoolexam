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
using SchoolExam.Infrastructure.Extensions;
using SchoolExam.Infrastructure.Specifications;

namespace SchoolExam.Infrastructure.Services;

public class ExamService : IExamService
{
    private static string _guidRegex = "[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}";
    private static string _pageQrCodeUri = "http://pageQrCode";
    
    private readonly ILogger<ExamService> _logger;
    private readonly ISchoolExamRepository _context;

    private readonly IRandomGenerator _randomGenerator;
    private readonly IQrCodeGenerator _qrCodeGenerator;
    private readonly IPdfService _pdfService;
    private readonly IQrCodeReader _qrCodeReader;

    public ExamService(ILogger<ExamService> logger, ISchoolExamRepository context,
        IRandomGenerator randomGenerator, IQrCodeGenerator qrCodeGenerator, IPdfService pdfService,
        IQrCodeReader qrCodeReader)
    {
        _logger = logger;
        _context = context;
        _randomGenerator = randomGenerator;
        _qrCodeGenerator = qrCodeGenerator;
        _pdfService = pdfService;
        _qrCodeReader = qrCodeReader;
    }

    public Exam? GetById(Guid examId)
    {
        var result = _context.Find<Exam, Guid>(examId);
        return result;
    }

    public IEnumerable<Exam> GetByTeacher(Guid teacherId)
    {
        var result = _context.List(new ExamByTeacherSpecification(teacherId));
        return result;
    }

    public IEnumerable<Exam> GetByStudent(Guid studentId)
    {
        throw new NotImplementedException();
    }

    public async Task Create(string title, string description, DateTime date, Guid courseId, Guid teacherId)
    {
        var examId = Guid.NewGuid();
        var exam = new Exam(examId, title, description, date, teacherId, courseId);

        _context.Add(exam);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Guid examId, string title, string description, DateTime date)
    {
        var exam = EnsureExamExists(new EntityByIdSpecification<Exam, Guid>(examId));
        exam.Title = title;
        exam.Description = description;
        exam.Date = date;

        _context.Update(exam);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Guid examId)
    {
        var exam = EnsureExamExists(new EntityByIdSpecification<Exam, Guid>(examId));
        if (exam.State.HasBeenBuilt())
        {
            throw new InvalidOperationException("An exam that already has been built must not be deleted.");
        }

        _context.Remove(exam);
        await _context.SaveChangesAsync();
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
            _context.Remove(exam.TaskPdfFile);
        }

        var taskPdfFile =
            new TaskPdfFile(Guid.NewGuid(), $"{examId.ToString()}.pdf", content.LongLength, DateTime.Now, userId,
                content, examId);
        // exam is ready to be built after having a task PDF file
        exam.State = ExamState.BuildReady;
        _context.Update(exam);
        _context.Add(taskPdfFile);
        await _context.SaveChangesAsync();
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
            _context.Remove(task);
        }

        var pdf = exam.TaskPdfFile!.Content;
        var links = _pdfService.GetUriLinkAnnotations(pdf);
        var tasksDict = tasks.ToDictionary(x => x.Id, x => x);
        var linkCandidates = links
            .Where(x => Regex.IsMatch(x.Uri, $"^task-{_guidRegex}$"));

        var matchedLinks = new List<PdfUriLinkAnnotationInfo>();
        var matchedTaskIds = new HashSet<Guid>();
        foreach (var link in linkCandidates)
        {
            var taskIdString = Regex.Match(link.Uri, $"{_guidRegex}$").Value;
            var taskId = Guid.Parse(taskIdString);
            if (tasksDict.ContainsKey(taskId))
            {
                if (matchedTaskIds.Contains(taskId))
                {
                    throw new InvalidOperationException($"Task with id {taskId} was found in PDF more than once.");
                }
                matchedTaskIds.Add(taskId);
                var task = tasksDict[taskId];
                matchedLinks.Add(link);
                var examTask = new ExamTask(Guid.NewGuid(), task.Title, task.MaxPoints, 1,
                    new ExamPosition(link.Page, link.Top));
                _context.Add(examTask);
                exam.Tasks.Add(examTask);
            }
        }

        if (matchedTaskIds.Count != tasks.Length)
        {
            var unmatchedTask = tasks.First(x => !matchedTaskIds.Contains(x.Id));
            throw new InvalidOperationException($"Task with id {unmatchedTask.Id} could not be found in PDF.");
        }

        var pdfWithoutTaskLinks = _pdfService.RemoveUriLinkAnnotations(pdf, matchedLinks.ToArray());
        _context.Remove(exam.TaskPdfFile);
        var newTaskPdfFile = new TaskPdfFile(Guid.NewGuid(), $"{examId}.pdf", pdfWithoutTaskLinks.LongLength,
            DateTime.Now, userId, pdfWithoutTaskLinks, examId);
        _context.Add(newTaskPdfFile);
        _context.Update(exam);
        
        await _context.SaveChangesAsync();
    }

    public async Task Build(Guid examId, int count, Guid userId)
    {
        if (count < 1)
        {
            throw new ArgumentException("At least one exam booklet must be built.");
        }

        var exam = EnsureExamExists(new ExamWithTaskPdfFileByIdSpecification(examId));
        if (exam.State.HasBeenBuilt())
        {
            throw new InvalidOperationException("Exam has already been built.");
        }

        if (exam.TaskPdfFile == null)
        {
            throw new InvalidOperationException("Exam does not have a task PDF file.");
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
            var qrCodes = qrCodeData.Select(x => _qrCodeGenerator.GeneratePngQrCode(x, 2)).ToArray();
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
            var booklet = new ExamBooklet(bookletId, examId, i + 1, bookletPdfFile);

            for (int page = 1; page <= pageCount; page++)
            {
                var bookletPage = new ExamBookletPage(Guid.NewGuid(), page, bookletId, qrCodeData[page - 1]);
                _context.Add(bookletPage);
            }

            _context.Add(booklet);
            _context.Add(bookletPdfFile);
        }

        exam.State = ExamState.SubmissionReady;
        _context.Update(exam);

        await _context.SaveChangesAsync();
    }

    public async Task Clean(Guid examId)
    {
        var exam = EnsureExamExists(new ExamWithBookletsByIdSpecification(examId));

        var submissionPages = _context.List(new SubmissionPageByExamSpecification(examId));
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
            _context.Remove(booklet);
        }

        exam.State = ExamState.BuildReady;
        _context.Update(exam);

        await _context.SaveChangesAsync();
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
        var exam = EnsureExamExists(new ExamWithBookletsWithPagesByIdSpecification(examId));

        if (exam.State != ExamState.SubmissionReady && exam.State != ExamState.InCorrection &&
            exam.State != ExamState.Corrected)
        {
            throw new InvalidOperationException("Exam is not ready to match submissions.");
        }

        var pages = _pdfService.Split(pdf);

        var submissions = _context.List<Submission>().ToDictionary(x => x.BookletId, x => x);
        var updatedSubmissionIds = new List<Guid>();
        for (int page = 1; page <= pages.Count; page++)
        {
            var pagePdf = pages[page - 1];

            var submissionPageId = Guid.NewGuid();
            var submissionPagePdf = new SubmissionPagePdfFile(Guid.NewGuid(), $"{page}.pdf", pagePdf.LongLength,
                DateTime.Now, userId, pagePdf, submissionPageId);

            var images = _pdfService.ParseImages(pagePdf).ToList();
            var qrCodes = images.SelectMany(x => _qrCodeReader.ReadQrCodes(x.Data, x.RotationMatrix)).ToList();

            var bookletPages = exam.Booklets.SelectMany(x => x.Pages);
            var matchedQrCode =
                qrCodes.SingleOrDefault(qrCode => bookletPages.Any(x => x.QrCodeData.Equals(qrCode.Data)));
            if (matchedQrCode != null)
            {
                // rotate PDF of submission page according to added QR code on page
                submissionPagePdf.Content = _pdfService.Rotate(submissionPagePdf.Content, -matchedQrCode.Degrees);

                var matchedPage = bookletPages.Single(x => x.QrCodeData.Equals(matchedQrCode.Data));

                // get existing submission for booklet
                if (!submissions.ContainsKey(matchedPage.BookletId))
                {
                    // create new submission if there has not been added one yet
                    var newSubmission = new Submission(Guid.NewGuid(), null, matchedPage.BookletId);
                    submissions.Add(newSubmission.BookletId, newSubmission);
                    _context.Add(newSubmission);
                }

                var submission = submissions[matchedPage.BookletId];
                updatedSubmissionIds.Add(submission.Id);

                // check if page has already been matched previously
                if (matchedPage.SubmissionPage == null)
                {
                    var submissionPage = new SubmissionPage(submissionPageId, examId, submissionPagePdf, submission.Id,
                        matchedPage.Id);
                    submission.Pages.Add(submissionPage);
                    _context.Add(submissionPage);
                }
                else
                {
                    _logger.LogInformation(
                        $"Exam booklet page corresponding to {page} of PDF document has already been matched. The previously matched PDF page was replaced by the most recently parsed page.");

                    var previouslyMatchedPage = submission.Pages.Single(x => x.BookletPageId.Equals(matchedPage.Id));
                    _context.Remove(previouslyMatchedPage.PdfFile);
                    previouslyMatchedPage.PdfFile = submissionPagePdf;
                }
            }
            else
            {
                _logger.LogWarning(
                    $"Page {page} of PDF document could not be matched to an existing exam booklet page.");

                // persist unmatched submission pages such that they can be matched manually afterwards
                var submissionPage = new SubmissionPage(submissionPageId, examId, submissionPagePdf, null, null);
                _context.Add(submissionPage);
            }

            _context.Add(submissionPagePdf);
        }

        await _context.SaveChangesAsync();
        if (updatedSubmissionIds.Count > 0)
        {
            await MergeCompleteSubmissions(examId, updatedSubmissionIds, userId);
            await CheckCompletenessOfExamSubmissions(examId);
        }
    }

    public IEnumerable<SubmissionPage> GetUnmatchedSubmissionPages(Guid examId)
    {
        EnsureExamExists(new EntityByIdSpecification<Exam, Guid>(examId));
        var submissionPages = _context.List(new SubmissionPageByExamSpecification(examId));

        var result = submissionPages.Where(x => !x.IsMatched);
        return result;
    }

    public IEnumerable<ExamBookletPage> GetUnmatchedBookletPages(Guid examId)
    {
        var exam = EnsureExamExists(new ExamWithBookletsWithPagesWithSubmissionPageByIdSpecification(examId));
        var bookletPages = exam.Booklets.SelectMany(x => x.Pages);

        var result = bookletPages.Where(x => !x.IsMatched);
        return result;
    }

    public async Task MatchManually(Guid examId, Guid bookletPageId, Guid submissionPageId, Guid userId)
    {
        EnsureExamExists(new EntityByIdSpecification<Exam, Guid>(examId));
        var bookletPage = _context.Find<ExamBookletPage, Guid>(bookletPageId);
        if (bookletPage == null)
        {
            throw new ArgumentException("Booklet page does not exist.");
        }

        var bookletExamId = _context.Find<ExamBooklet, Guid>(bookletPage.BookletId)!.ExamId;
        if (!examId.Equals(bookletExamId))
        {
            throw new InvalidOperationException("Booklet page is not part of the exam.");
        }

        var submissionPage = _context.Find<SubmissionPage, Guid>(submissionPageId);
        if (submissionPage == null)
        {
            throw new ArgumentException("Submission page does not exist.");
        }

        if (!examId.Equals(submissionPage.ExamId))
        {
            throw new InvalidOperationException("Submission page is not part of the exam.");
        }

        var bookletPageMatched = _context.List(new SubmissionPageByBookletPageSpecification(bookletPageId)).Any();

        // not possible to manually match a previously matched pages
        if (bookletPageMatched || submissionPage.BookletPageId.HasValue)
        {
            throw new InvalidOperationException(
                "The booklet page and/or the submission page have already been matched.");
        }

        var bookletId = bookletPage.BookletId;
        var submission = _context.Find(new SubmissionByBookletSpecification(bookletId)) ??
                         new Submission(Guid.NewGuid(), null, bookletId);

        submissionPage.SubmissionId = submission.Id;
        submissionPage.BookletPageId = bookletPageId;

        _context.Update(submissionPage);
        await _context.SaveChangesAsync();

        await MergeCompleteSubmissions(examId, new[] {submission.Id}, userId);
        await CheckCompletenessOfExamSubmissions(examId);
    }
    
    private Exam EnsureExamExists(EntityByIdSpecification<Exam, Guid> spec)
    {
        var exam = _context.Find(spec);
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
            _context.List(new SubmissionWithPdfFileAndPagesWithPdfFileByIdsSpecification(updatedSubmissionIdsSet));
        var updatedSubmissionsDict = updatedSubmissions.ToDictionary(x => x.BookletId, x => x);
        var exam = _context.Find(new ExamWithTasksAndBookletsWithPagesWithSubmissionPageByIdSpecification(examId))!;

        // get updated booklets with a complete submission
        var booklets = exam.Booklets
            .Where(x => x.HasCompleteSubmission)
            .Where(x => updatedSubmissionsDict.ContainsKey(x.Id)).ToList();
        var bookletPagesDict = booklets.SelectMany(x => x.Pages).ToDictionary(x => x.Id, x => x.Page);
        
        // prepare outline to be added to all complete submission PDFs
        var outlineElements = exam.Tasks.Select(x => new PdfOutlineInfo(x.Title, x.Position.Page, (float) x.Position.Y))
            .ToArray();

        foreach (var booklet in booklets)
        {
            var submission = updatedSubmissionsDict[booklet.Id];
            if (submission.PdfFile != null)
            {
                _context.Remove(submission.PdfFile);
            }

            // merge submission pages of booklet ordered page numbers
            var pages = submission.Pages.OrderBy(x => bookletPagesDict[x.BookletPageId!.Value])
                .Select(x => x.PdfFile.Content).ToArray();
            var submissionPdf = _pdfService.Merge(pages);
            var submissionPdfWithOutline = _pdfService.SetTopLevelOutline(submissionPdf, outlineElements);

            var submissionPdfFile = new SubmissionPdfFile(Guid.NewGuid(), $"submission_{submission.Id}.pdf",
                submissionPdfWithOutline.LongLength, DateTime.Now, userId, submissionPdfWithOutline, submission.Id);
            _context.Add(submissionPdfFile);
        }

        await _context.SaveChangesAsync();
    }

    private async Task CheckCompletenessOfExamSubmissions(Guid examId)
    {
        var exam = _context.Find(new ExamWithBookletsWithPagesWithSubmissionPageByIdSpecification(examId))!;
        var hasSubmission = exam.Booklets.Any(x => x.HasCompleteSubmission);
        exam.State = hasSubmission ? ExamState.InCorrection : ExamState.SubmissionReady;

        _context.Update(exam);
        await _context.SaveChangesAsync();
    }
}