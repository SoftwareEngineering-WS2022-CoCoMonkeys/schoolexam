namespace SchoolExam.Application.QrCode;

public interface IQrCodeGenerator
{
    (byte[] qrCode, byte[] pngQrCode) GeneratePngQrCode(string data, int pixelsPerModule);
}