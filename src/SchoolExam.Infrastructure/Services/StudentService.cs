using SchoolExam.Application.Pdf;
using SchoolExam.Application.QrCode;
using SchoolExam.Application.Repository;
using SchoolExam.Application.Services;
using SchoolExam.Application.TagLayout;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Infrastructure.Extensions;

namespace SchoolExam.Infrastructure.Services;

public class StudentService : IStudentService
{
    private readonly ISchoolExamRepository _repository;
    private readonly IQrCodeGenerator _qrCodeGenerator;
    private readonly IPdfService _pdfService;

    public StudentService(ISchoolExamRepository repository, IQrCodeGenerator qrCodeGenerator, IPdfService pdfService)
    {
        _repository = repository;
        _qrCodeGenerator = qrCodeGenerator;
        _pdfService = pdfService;
    }

    public byte[] GenerateQrCodeSheetForStudent<TLayout>(Guid studentId) where TLayout : ITagLayout<TLayout>, new()
    {
        var student = _repository.Find<Student, Guid>(studentId);
        if (student == null)
        {
            throw new ArgumentException("Student does not exist.");
        }

        var qrCode = _qrCodeGenerator.GeneratePngQrCode(student.QrCode.Data);

        var layout = new TLayout();
        // width and height are always equal for a QR code
        var qrCodeSize = Math.Min(layout.TagSize.Width, layout.TagSize.Height) - 2 * layout.Padding;
        var qrCodeLeft = (layout.TagSize.Width - qrCodeSize) / 2;
        var qrCodeBottom = (layout.TagSize.Height - qrCodeSize) / 2;

        var elements = layout.GetElements();
        var images = elements.Select(x => new PdfImageRenderInfo(1, x.Left + qrCodeLeft,
            layout.PageSize.Height - x.Top - layout.TagSize.Height + qrCodeBottom, qrCodeSize, qrCode)).ToArray();

        var pdf = _pdfService.CreateEmptyPdf(1, layout.PageSize);
        var pdfWithQrCodes =  _pdfService.RenderImages(pdf, images);

        return pdfWithQrCodes;
    }
}