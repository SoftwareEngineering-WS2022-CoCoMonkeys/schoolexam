using System.Text.RegularExpressions;
using SchoolExam.Application.Pdf;
using SchoolExam.Application.QrCode;
using SchoolExam.Application.RandomGenerator;
using SchoolExam.Application.Repository;
using SchoolExam.Application.Services;
using SchoolExam.Application.Specifications;
using SchoolExam.Application.TagLayout;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Exceptions;
using SchoolExam.Domain.Extensions;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Infrastructure.Specifications;

namespace SchoolExam.Infrastructure.Services;

public class ExamBuildService : ExamServiceBase, IExamBuildService
{
    private static string _pageQrCodeUri = "http://pageQrCode";
    
    private readonly IPdfService _pdfService;
    private readonly IRandomGenerator _randomGenerator;
    private readonly IQrCodeGenerator _qrCodeGenerator;

    public ExamBuildService(ISchoolExamRepository repository, IPdfService pdfService, IRandomGenerator randomGenerator,
        IQrCodeGenerator qrCodeGenerator)
        : base(repository)
    {
        _pdfService = pdfService;
        _randomGenerator = randomGenerator;
        _qrCodeGenerator = qrCodeGenerator;
    }

    public async Task<int> Build(Guid examId, Guid userId)
    {
        var exam = EnsureExamExists(new ExamWithTaskPdfFileAndParticipantsById(examId));
        var examStudentIds = exam.Participants.OfType<ExamStudent>().Select(x => x.ParticipantId);
        var examCourseStudentIds = exam.Participants.OfType<ExamCourse>().SelectMany(x => x.Course.Students)
            .Select(x => x.StudentId);
        var studentIds = examStudentIds.Union(examCourseStudentIds).Distinct();
        var count = studentIds.Count();
        if (count < 1)
        {
            throw new DomainException("At least one exam booklet must be built.");
        }

        if (exam.TaskPdfFile == null)
        {
            throw new DomainException("Exam does not have a task PDF file.");
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
                Repository.Add(bookletPage);
            }

            Repository.Add(booklet);
            Repository.Add(bookletPdfFile);
        }

        exam.State = ExamState.SubmissionReady;
        Repository.Update(exam);

        await Repository.SaveChangesAsync();

        return count;
    }

    public byte[] GetParticipantQrCodePdf<TLayout>(Guid examId) where TLayout : ITagLayout<TLayout>, new()
    {
        EnsureExamExists(new EntityByIdSpecification<Exam>(examId));

        var students = GetStudentsByExam(examId).ToArray();

        var layout = new TLayout();
        var elements = layout.GetElements().ToArray();

        var textHeight = PdfUnitConverter.ConvertMmToPoint(5);
        // width and height are always equal for a QR code
        var qrCodeSize = Math.Min(layout.TagSize.Width, layout.TagSize.Height - textHeight - layout.Padding) -
                         2 * layout.Padding;
        var qrCodeLeft = (layout.TagSize.Width - qrCodeSize) / 2;
        var qrCodeBottom = (layout.TagSize.Height - qrCodeSize - layout.Padding - textHeight) / 2 + layout.Padding +
                           textHeight;

        var images = new List<PdfImageRenderInfo>();
        var texts = new List<PdfTextRenderInfo>();
        for (int i = 0; i < students.Length; i++)
        {
            var student = students[i];
            var qrCode = _qrCodeGenerator.GeneratePngQrCode(student.QrCode.Data);
            var page = i / elements.Length + 1;

            var element = elements[i % elements.Length];
            var left = element.Left + qrCodeLeft;
            var bottom = layout.PageSize.Height - element.Top - layout.TagSize.Height + qrCodeBottom;
            images.Add(new PdfImageRenderInfo(page, left, bottom, qrCodeSize, qrCode));

            var studentName = $"{student.FirstName} {student.LastName}";
            var leftText = element.Left + layout.Padding;
            var bottomText = layout.PageSize.Height - element.Top - layout.TagSize.Height + layout.Padding;
            var widthText = layout.TagSize.Width - 2 * layout.Padding;
            var heightText = textHeight;
            texts.Add(new PdfTextRenderInfo(studentName, page, leftText, bottomText, widthText, heightText));
        }

        var pdf = _pdfService.CreateEmptyPdf(1, layout.PageSize);
        var pdfWithQrCodes = _pdfService.RenderImages(pdf, images.ToArray());
        var pdfWithTexts = _pdfService.RenderTexts(pdfWithQrCodes, texts.ToArray());

        return pdfWithTexts;
    }

    public async Task Clean(Guid examId)
    {
        var exam = EnsureExamExists(new ExamWithBookletsByIdSpecification(examId));

        var submissionPages = Repository.List(new SubmissionPageByExamSpecification(examId));
        if (submissionPages.Any())
        {
            throw new DomainException("An exam with existing submission pages must not be cleaned.");
        }

        if (!exam.State.HasBeenBuilt())
        {
            throw new DomainException("The exam has not been built yet.");
        }

        var booklets = exam.Booklets;
        foreach (var booklet in booklets)
        {
            Repository.Remove(booklet);
        }

        exam.State = ExamState.BuildReady;
        Repository.Update(exam);

        await Repository.SaveChangesAsync();
    }

    public byte[] GetConcatenatedBookletPdfFile(Guid examId)
    {
        var exam = EnsureExamExists(new ExamWithBookletsWithPdfFileByIdSpecification(examId));
        var pdfs = exam.Booklets.OrderBy(x => x.SequenceNumber).Select(x => x.PdfFile.Content).ToArray();
        var result = _pdfService.Merge(pdfs);
        return result;
    }
}