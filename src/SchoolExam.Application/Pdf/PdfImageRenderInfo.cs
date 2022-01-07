namespace SchoolExam.Application.Pdf;

public class PdfImageRenderInfo : PdfRenderInfoBase
{
    public float Width { get; }
    public byte[] Data { get; }

    public PdfImageRenderInfo(int page, float left, float bottom, float width, byte[] data) : base(page, left, bottom)
    {
        Width = width;
        Data = data;
    }
}