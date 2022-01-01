namespace SchoolExam.Application.Pdf;

public interface IPdfService
{
    int GetNumberOfPages(byte[] pdf);
    byte[] RenderImages(byte[] pdf, params PdfImageRenderInfo[] images);
    byte[] RenderTexts(byte[] pdf, params PdfTextRenderInfo[] texts);
    IEnumerable<PdfImageParseInfo> ParseImages(byte[] pdf);
    IList<byte[]> Split(byte[] pdf);
    byte[] Merge(params byte[][] pdfs);
    bool Compare(byte[] first, byte[] second);
    DateTime GetModificationDate(byte[] pdf);
}