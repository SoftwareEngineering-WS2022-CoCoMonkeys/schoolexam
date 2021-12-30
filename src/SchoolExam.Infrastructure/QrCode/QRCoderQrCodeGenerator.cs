using System.Text;
using QRCoder;
using SchoolExam.Application.QrCode;

namespace SchoolExam.Infrastructure.QrCode;

public class QRCoderQrCodeGenerator : IQrCodeGenerator
{
    private readonly QRCodeGenerator _qrCodeGenerator;
    public QRCoderQrCodeGenerator()
    {
        _qrCodeGenerator = new QRCodeGenerator();
    }

    public byte[] GeneratePngQrCode(string message, int pixelsPerModule)
    {
        var data = Encoding.ASCII.GetBytes(message);
        var qrCodeData = _qrCodeGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.H);
        var pngByteQrCode = new PngByteQRCode(qrCodeData);
        return pngByteQrCode.GetGraphic(pixelsPerModule);
    }
}