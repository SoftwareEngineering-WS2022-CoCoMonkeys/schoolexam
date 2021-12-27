namespace SchoolExam.Application.QrCode;

public interface IQrCodeGenerator
{
    byte[] GeneratePngQrCode(byte[] data, int pixelsPerModule);
}