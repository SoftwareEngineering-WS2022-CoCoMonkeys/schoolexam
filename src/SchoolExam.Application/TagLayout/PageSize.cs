namespace SchoolExam.Application.TagLayout;

public class PageSize : Size
{
    public static PageSize A4 => FromMm(210, 297);

    public PageSize(float width, float height) : base(width, height)
    {
    }
    
    public static PageSize FromMm(float width, float height)
    {
        return new PageSize(PdfUnitConverter.ConvertMmToPoint(width), PdfUnitConverter.ConvertMmToPoint(height));
    }
}