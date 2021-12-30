namespace SchoolExam.Application.QrCode;

public class QrCodeParseInfo
{
    public int Page { get; }
    public string Data { get; }

    public QrCodeParseInfo(int page, string data)
    {
        Page = page;
        Data = data;
    }
}