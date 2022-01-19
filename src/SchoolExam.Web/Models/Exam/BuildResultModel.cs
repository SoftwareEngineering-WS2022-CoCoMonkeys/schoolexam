namespace SchoolExam.Web.Models.Exam;

public class BuildResultModel
{
    public int Count { get; set; }
    public string PdfFile { get; set; } = null!;
    public string QrCodePdfFile { get; set; } = null!;
}