using Microsoft.Extensions.Logging;
using SchoolExam.Application.Pdf;
using SchoolExam.Application.QrCode;
using SchoolExam.Application.Repository;
using SchoolExam.Application.Services;
using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;
using SchoolExam.Domain.Exceptions;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Extensions;
using SchoolExam.Infrastructure.Extensions;
using SchoolExam.Infrastructure.Specifications;

namespace SchoolExam.Infrastructure.Services;

public class MatchingService : ExamServiceBase, IMatchingService
{
    private readonly IPdfService _pdfService;
    private readonly IQrCodeReader _qrCodeReader;
    private readonly ILogger<MatchingService> _logger;

    public MatchingService(ISchoolExamRepository repository, IPdfService pdfService, IQrCodeReader qrCodeReader,
        ILogger<MatchingService> logger) : base(repository)
    {
        _pdfService = pdfService;
        _qrCodeReader = qrCodeReader;
        _logger = logger;
    }

    public async Task Match(Guid examId, byte[] pdf, Guid userId)
    {
        var exam = EnsureExamExists(
            new ExamWithBookletsWithPagesWithSubmissionPageWithPdfFileByIdSpecification(examId));

        if (exam.State != ExamState.SubmissionReady && exam.State != ExamState.InCorrection &&
            exam.State != ExamState.Corrected)
        {
            throw new DomainException("Exam is not ready to match submissions.");
        }

        var pages = _pdfService.Split(pdf);

        var submissions = Repository.List<Submission, SubmissionWithPagesSpecification>()
            .ToDictionary(x => x.BookletId, x => x);

        // get participating students
        var students = GetStudentsByExam(examId);
        var studentsDict = students.ToDictionary(x => x.QrCode.Data, x => x);

        var updatedSubmissionIds = new List<Guid>();
        for (int page = 1; page <= pages.Count; page++)
        {
            var pagePdf = pages[page - 1];

            // extract QR codes from submission page
            var images = _pdfService.ParseImages(pagePdf).ToList();
            var qrCodes = images.SelectMany(x => _qrCodeReader.ReadQrCodes(x.Data, x.RotationMatrix)).ToList();
            // find student identifier QR code
            var matchedStudentQrCode = qrCodes.SingleOrDefault(qrCode => studentsDict.ContainsKey(qrCode.Data))?.Data;
            var studentQrCode = matchedStudentQrCode != null
                ? new SchoolExam.Domain.ValueObjects.QrCode(matchedStudentQrCode)
                : null;
            // find student with identifier QR code
            var student = matchedStudentQrCode != null ? studentsDict[matchedStudentQrCode] : null;

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
                    Repository.Add(newSubmission);
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
                    Repository.Add(submissionPage);
                }
                else
                {
                    _logger.LogInformation(
                        $"Exam booklet page corresponding to {page} of PDF document has already been matched. The previously matched PDF page was replaced by the most recently parsed page.");

                    var previouslyMatchedPage = submission.Pages.Single(x => x.BookletPageId.Equals(matchedPage.Id));
                    Repository.Remove(previouslyMatchedPage.PdfFile);
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
                Repository.Add(submissionPage);
            }

            Repository.Add(submissionPagePdf);
        }

        await Repository.SaveChangesAsync();
        if (updatedSubmissionIds.Count > 0)
        {
            await MergeCompleteSubmissions(examId, updatedSubmissionIds, userId);
            await CheckCompletenessOfExamSubmissions(examId);
        }
    }

    public IEnumerable<SubmissionPage> GetUnmatchedSubmissionPages(Guid examId)
    {
        EnsureExamExists(new EntityByIdSpecification<Exam>(examId));
        var submissionPages = Repository.List(new SubmissionPageByExamSpecification(examId));

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
        EnsureExamExists(new EntityByIdSpecification<Exam>(examId));
        var bookletPage = Repository.Find<BookletPage>(bookletPageId);
        if (bookletPage == null)
        {
            throw new DomainException("Booklet page does not exist.");
        }

        var bookletExamId = Repository.Find<Booklet>(bookletPage.BookletId)!.ExamId;
        if (!examId.Equals(bookletExamId))
        {
            throw new DomainException("Booklet page is not part of the exam.");
        }

        var submissionPage = Repository.Find<SubmissionPage>(submissionPageId);
        if (submissionPage == null)
        {
            throw new DomainException("Submission page does not exist.");
        }

        if (!examId.Equals(submissionPage.ExamId))
        {
            throw new DomainException("Submission page is not part of the exam.");
        }

        var bookletPageMatched = Repository.List(new SubmissionPageByBookletPageSpecification(bookletPageId)).Any();

        // not possible to manually match a previously matched pages
        if (bookletPageMatched || submissionPage.BookletPageId.HasValue)
        {
            throw new DomainException(
                "The booklet page and/or the submission page have already been matched.");
        }

        var bookletId = bookletPage.BookletId;
        var submission = Repository.Find(new SubmissionByBookletSpecification(bookletId)) ??
                         new Submission(Guid.NewGuid(), null, bookletId, DateTime.Now.SetKindUtc());

        var student = submissionPage.StudentQrCode != null
            ? Repository.Find(new StudentByQrCodeSpecification(submissionPage.StudentQrCode.Data))
            : null;
        AssignStudentToSubmission(student?.Id, submission);

        submissionPage.SubmissionId = submission.Id;
        submissionPage.BookletPageId = bookletPageId;
        submission.UpdatedAt = DateTime.Now.SetKindUtc();

        Repository.Update(submissionPage);
        await Repository.SaveChangesAsync();

        await MergeCompleteSubmissions(examId, new[] {submission.Id}, userId);
        await CheckCompletenessOfExamSubmissions(examId);
    }
    
    private async Task MergeCompleteSubmissions(Guid examId, IEnumerable<Guid> updatedSubmissionIds, Guid userId)
    {
        var updatedSubmissionIdsSet = updatedSubmissionIds.ToHashSet();
        var updatedSubmissions =
            Repository.List(
                new SubmissionWithAnswersAndPdfFileAndPagesWithPdfFileByIdsSpecification(updatedSubmissionIdsSet));
        var updatedSubmissionsDict = updatedSubmissions.ToDictionary(x => x.BookletId, x => x);
        var exam = Repository.Find(new ExamWithTasksAndBookletsWithPagesWithSubmissionPageByIdSpecification(examId))!;

        // get updated booklets with a complete submission
        var booklets = exam.Booklets
            .Where(x => x.HasCompleteSubmission)
            .Where(x => updatedSubmissionsDict.ContainsKey(x.Id)).ToList();
        var bookletPagesDict = booklets.SelectMany(x => x.Pages).ToDictionary(x => x.Id, x => x.Page);

        // prepare outline to be added to all complete submission PDFs
        var outlineElements = exam.Tasks.Select(x => new PdfOutlineInfo(x.Title, x.Start.Page, (float) x.Start.Y))
            .ToArray();
        
        foreach (var booklet in booklets)
        {
            var submission = updatedSubmissionsDict[booklet.Id];
            if (submission.PdfFile != null)
            {
                Repository.Remove(submission.PdfFile);
            }

            // remove previously existing
            foreach (var answer in submission.Answers)
            {
                Repository.Remove(answer);
            }

            // add answers for all tasks to submission
            foreach (var task in exam.Tasks)
            {
                var answer = new Answer(Guid.NewGuid(), task.Id, submission.Id, AnswerState.Pending, null,
                    DateTime.Now.SetKindUtc());
                var defaultSegmentAnswer = new AnswerSegment(Guid.NewGuid(),
                    new ExamPosition(task.Start.Page, task.Start.Y), new ExamPosition(task.End.Page, task.End.Y),
                    answer.Id);
                Repository.Add(answer);
                Repository.Add(defaultSegmentAnswer);
            }

            // merge submission pages of booklet ordered page numbers
            var pages = submission.Pages.OrderBy(x => bookletPagesDict[x.BookletPageId!.Value])
                .Select(x => x.PdfFile.Content).ToArray();
            var submissionPdf = _pdfService.Merge(pages);
            var submissionPdfWithOutline = _pdfService.SetTopLevelOutline(submissionPdf, outlineElements);

            var submissionPdfFile = new SubmissionPdfFile(Guid.NewGuid(), $"submission_{submission.Id}.pdf",
                submissionPdfWithOutline.LongLength, DateTime.Now, userId, submissionPdfWithOutline, submission.Id);
            Repository.Add(submissionPdfFile);
        }

        await Repository.SaveChangesAsync();
    }

    private async Task CheckCompletenessOfExamSubmissions(Guid examId)
    {
        var exam = Repository.Find(new ExamWithBookletsWithPagesWithSubmissionPageByIdSpecification(examId))!;
        var hasSubmission = exam.Booklets.Any(x => x.HasCompleteSubmission);
        exam.State = hasSubmission ? ExamState.InCorrection : ExamState.SubmissionReady;

        Repository.Update(exam);
        await Repository.SaveChangesAsync();
    }

    private void AssignStudentToSubmission(Guid? studentId, Submission submission)
    {
        // check if submission has already been assigned to another student
        if (submission.StudentId.HasValue && studentId.HasValue && !studentId.Equals(submission.StudentId))
        {
            throw new DomainException(
                $"Submission cannot be assigned to student with identifier {studentId} because it was previously assigned to student with identifier {submission.StudentId.Value}.");
        }

        // assign student to submission
        submission.StudentId ??= studentId;
    }
}