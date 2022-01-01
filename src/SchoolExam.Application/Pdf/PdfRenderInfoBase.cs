namespace SchoolExam.Application.Pdf;

public abstract class PdfRenderInfoBase
{
    public int Page { get; }
    public float Left { get; }
    public float Bottom { get; }

    public PdfRenderInfoBase(int page, float left, float bottom)
    {
        Page = page;
        Left = left;
        Bottom = bottom;
    }
}