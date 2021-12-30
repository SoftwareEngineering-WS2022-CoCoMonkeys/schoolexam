using Microsoft.Extensions.Logging;
using SchoolExam.Application.DataContext;
using SchoolExam.Application.Pdf;
using SchoolExam.Application.QrCode;
using SchoolExam.Application.RandomGenerator;
using SchoolExam.Application.Repositories;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Infrastructure.Repositories;

public class ExamRepository : IExamRepository
{
    private readonly ILogger<ExamRepository> _logger;
    private readonly ISchoolExamDataContext _context;

    private readonly IRandomGenerator _randomGenerator;
    private readonly IQrCodeGenerator _qrCodeGenerator;
    private readonly IPdfService _pdfService;
    private readonly IQrCodeReader _qrCodeReader;

    public ExamRepository(ILogger<ExamRepository> logger, ISchoolExamDataContext context,
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

    public async Task SetTaskPdfFile(Guid examId, string name, Guid userId, byte[] content)
    {
        var exam = EnsureExamExists(examId);

        // remove previously existing task PDF file
        if (exam.TaskPdfFile != null)
        {
            _context.Remove(exam.TaskPdfFile);
        }

        var taskPdfFile =
            new TaskPdfFile(Guid.NewGuid(), name, content.LongLength, DateTime.Now, userId, content, examId);
        _context.Add(taskPdfFile);
        await _context.SaveChangesAsync();
    }

    public async Task Build(Guid examId, int count, Guid userId)
    {
        if (count < 1)
        {
            throw new ArgumentException("At least one exam booklet must be built.");
        }

        var exam = EnsureExamExists(examId);
        if (exam.HasBeenBuilt)
        {
            throw new InvalidOperationException("Exam has already been built.");
        }

        if (exam.TaskPdfFile == null)
        {
            throw new InvalidOperationException("Exam does not have a task PDF file.");
        }

        var content = exam.TaskPdfFile.Content;
        var pageCount = _pdfService.GetNumberOfPages(content);
        // generate exam booklets
        for (int i = 0; i < count; i++)
        {
            var qrCodeData = Enumerable.Range(0, pageCount).Select(x => _randomGenerator.GenerateHexString(32))
                .ToArray();
            var qrCodes = qrCodeData.Select(x => _qrCodeGenerator.GeneratePngQrCode(x, 2)).ToArray();
            var pdfImageInfos = Enumerable.Range(1, pageCount)
                .Select(x => new PdfImageRenderInfo(x, 10.0f, 10.0f, 42.0f, qrCodes[x - 1])).ToArray();
            var bookletContent = _pdfService.RenderImages(content, pdfImageInfos);

            var bookletId = Guid.NewGuid();
            var bookletPdfFile = new BookletPdfFile(Guid.NewGuid(), $"{i + 1}.pdf", bookletContent.LongLength,
                DateTime.Now, userId, bookletContent, bookletId);
            var booklet = new ExamBooklet(bookletId, bookletPdfFile, examId);
            
            for (int page = 1; page <= pageCount; page++)
            {
                var bookletPage = new ExamBookletPage(Guid.NewGuid(), page, booklet.Id, qrCodeData[page - 1]);
                _context.Add(bookletPage);
            }

            _context.Add(booklet);
            _context.Add(bookletPdfFile);
        }

        await _context.SaveChangesAsync();
    }

    public async Task Match(Guid examId, byte[] pdf, Guid userId)
    {
        var exam = EnsureExamExists(examId);

        var pages = _pdfService.Split(pdf);

        var submissions = _context.Submissions.ToDictionary(x => x.BookletId, x => x);
        for (int page = 1; page <= pages.Count; page++)
        {
            var pagePdf = pages[page - 1];

            var submissionPageId = Guid.NewGuid();
            var submissionPagePdf = new SubmissionPagePdfFile(Guid.NewGuid(), $"{page}.pdf", pagePdf.LongLength,
                DateTime.Now, userId, pagePdf, submissionPageId);
            
            var images = _pdfService.ParseImages(pagePdf).ToList();
            var qrCodes = images.SelectMany(x => _qrCodeReader.ReadQrCodes(x.Data)).ToList();

            var matchedQrCode = qrCodes.SingleOrDefault(qrCode =>
                _context.ExamBookletPages.SingleOrDefault(x => x.QrCodeData.Equals(qrCode)) != null);
            if (matchedQrCode != null)
            {
                var matchedPage = _context.ExamBookletPages.Single(x => x.QrCodeData.Equals(matchedQrCode));

                // get existing submission for booklet
                if (!submissions.ContainsKey(matchedPage.BookletId))
                {
                    // create new submission if there has not been added one yet
                    var newSubmission = new Submission(Guid.NewGuid(), null, matchedPage.BookletId);
                    submissions.Add(newSubmission.BookletId, newSubmission);
                    _context.Add(newSubmission);
                }

                var submission = submissions[matchedPage.BookletId];

                // check if page has already been matched previously
                if (submission.Pages.All(x => x.Page != matchedPage.Page))
                {
                    var submissionPage =
                        new SubmissionPage(submissionPageId, examId, submissionPagePdf, submission.Id, matchedPage.Page);
                    submission.Pages.Add(submissionPage);
                    _context.Add(submissionPagePdf);
                    _context.Add(submissionPage);
                }
                else
                {
                    _logger.LogInformation(
                        $"Exam booklet page corresponding to {page} of PDF document has already been matched.");
                }
            }
            else
            {
                _logger.LogWarning(
                    $"Page {page} of PDF document could not be matched to an existing exam booklet page.");
                
                // persist unmatched submission pages such that they can be matched manually afterwards
                var submissionPage = new SubmissionPage(submissionPageId, examId, submissionPagePdf, null, null);
                _context.Add(submissionPage);
                _context.Add(submissionPagePdf);
            }
        }

        await _context.SaveChangesAsync();
    }

    private Exam EnsureExamExists(Guid examId)
    {
        var exam = _context.Exams.SingleOrDefault(x => x.Id.Equals(examId));
        if (exam == null)
        {
            throw new ArgumentException("Exam does not exist");
        }

        return exam;
    }
}