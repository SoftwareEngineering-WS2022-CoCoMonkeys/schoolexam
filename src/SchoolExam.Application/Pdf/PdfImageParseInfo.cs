namespace SchoolExam.Application.Pdf;

public class PdfImageParseInfo
{
    public int Page { get; }
    public byte[] Data { get; }

    public PdfImageParseInfo(int page, byte[] data)
    {
        Page = page;
        Data = data;
    }
}