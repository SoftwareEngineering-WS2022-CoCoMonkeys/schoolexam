using System.Text;
using QRCoder;
using SchoolExam.Application.QrCode;

namespace SchoolExam.Infrastructure.QrCode;

public class QRCoderQrCodeGenerator : IQrCodeGenerator
{
    private readonly QRCodeGenerator _qrCodeGenerator;
    private readonly int _pixelsPerModule = 5;
    
    public QRCoderQrCodeGenerator()
    {
        _qrCodeGenerator = new QRCodeGenerator();
    }

    public byte[] GeneratePngQrCode(string message)
    {
        var data = Encoding.ASCII.GetBytes(message);
        var qrCodeData = _qrCodeGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.H);
        var pngByteQrCode = new PngByteQRCode(qrCodeData);
        return pngByteQrCode.GetGraphic(_pixelsPerModule);
    }
}