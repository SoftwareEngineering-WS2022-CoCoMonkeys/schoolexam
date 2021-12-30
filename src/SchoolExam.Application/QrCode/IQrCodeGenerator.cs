namespace SchoolExam.Application.QrCode;

public interface IQrCodeGenerator
{
    byte[] GeneratePngQrCode(string message, int pixelsPerModule);
}