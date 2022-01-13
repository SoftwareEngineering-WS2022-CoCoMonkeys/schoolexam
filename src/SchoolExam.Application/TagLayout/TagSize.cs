namespace SchoolExam.Application.TagLayout;

public class TagSize : Size
{
    public TagSize(float width, float height) : base(width, height)
    {
    }

    public static TagSize FromMm(float width, float height)
    {
        return new TagSize(PdfUnitConverter.ConvertMmToPoint(width), PdfUnitConverter.ConvertMmToPoint(height));
    }
}