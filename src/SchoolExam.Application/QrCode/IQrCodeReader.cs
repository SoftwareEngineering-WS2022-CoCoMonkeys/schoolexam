namespace SchoolExam.Application.QrCode;

public interface IQrCodeReader
{
    IEnumerable<string> ReadQrCodes(byte[] image);
}