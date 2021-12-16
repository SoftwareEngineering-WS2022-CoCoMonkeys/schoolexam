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

    public (byte[] qrCode, byte[] pngQrCode) GeneratePngQrCode(string data, int pixelsPerModule)
    {
        var qrCodeData = _qrCodeGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.H);
        var pngByteQrCode = new PngByteQRCode(qrCodeData);
        return (qrCodeData.GetRawData(QRCodeData.Compression.Uncompressed), pngByteQrCode.GetGraphic(pixelsPerModule));
    }
}