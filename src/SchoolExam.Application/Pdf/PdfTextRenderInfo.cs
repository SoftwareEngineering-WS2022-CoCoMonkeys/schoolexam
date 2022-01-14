namespace SchoolExam.Application.Pdf;

public class PdfTextRenderInfo : PdfRenderInfoBase
{
    public string Text { get; }
    public float Width { get; }
    public float Height { get; }
    public float FontSize { get; }

    public PdfTextRenderInfo(string text, int page, float left, float bottom, float width, float height, float fontSize)
        : base(page, left, bottom)
    {
        Text = text;
        Width = width;
        Height = height;
        FontSize = fontSize;
    }
}