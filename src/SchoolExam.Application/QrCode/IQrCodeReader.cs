using SchoolExam.Application.Pdf;

namespace SchoolExam.Application.QrCode;

public interface IQrCodeReader
{
    IEnumerable<QrCodeParseInfo> ReadQrCodes(byte[] image, RotationMatrix rotationMatrix);
}