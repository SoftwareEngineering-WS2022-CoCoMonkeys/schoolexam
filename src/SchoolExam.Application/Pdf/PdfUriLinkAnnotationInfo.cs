namespace SchoolExam.Application.Pdf;

public class PdfUriLinkAnnotationInfo
{
    public string Uri { get; }
    public int Page { get;  }
    public float Left { get; }
    public float Top { get; }
    public float Bottom { get; }
    public float Width { get; }

    public PdfUriLinkAnnotationInfo(string uri, int page, float left, float top, float bottom, float width)
    {
        Uri = uri;
        Page = page;
        Left = left;
        Top = top;
        Bottom = bottom;
        Width = width;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Uri, Page, Left, Top, Bottom, Width);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;
        if (obj is not PdfUriLinkAnnotationInfo other)
            return false;
        return Equals(other);
    }

    protected bool Equals(PdfUriLinkAnnotationInfo other)
    {
        return Uri.Equals(other.Uri) && Page.Equals(other.Page) && Left.Equals(other.Left) && Top.Equals(other.Top) &&
               Width.Equals(other.Width) && Bottom.Equals(other.Bottom);
    }
}