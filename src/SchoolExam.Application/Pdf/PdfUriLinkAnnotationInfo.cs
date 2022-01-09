namespace SchoolExam.Application.Pdf;

public class PdfUriLinkAnnotationInfo
{
    public string Uri { get; }
    public int Page { get;  }
    public float Y { get; }

    public PdfUriLinkAnnotationInfo(string uri, int page, float y)
    {
        Uri = uri;
        Page = page;
        Y = y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Uri, Page, Y);
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
        return Uri.Equals(other.Uri) && Page.Equals(other.Page) && Y.Equals(other.Y);
    }
}