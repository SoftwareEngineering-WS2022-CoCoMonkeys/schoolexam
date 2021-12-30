namespace SchoolExam.Application.Pdf;

public class PdfImageRenderInfo
{
    public int Page { get; }
    public float Left { get; }
    public float Bottom { get; }
    public float Width { get; }
    public byte[] Data { get; }
    
    public PdfImageRenderInfo(int page, float left, float bottom, float width, byte[] data)
    {
        Page = page;
        Left = left;
        Bottom = bottom;
        Width = width;
        Data = data;
    }
}