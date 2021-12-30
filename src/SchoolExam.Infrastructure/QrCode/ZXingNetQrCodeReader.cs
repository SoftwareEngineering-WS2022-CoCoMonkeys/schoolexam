using SchoolExam.Application.QrCode;
using SkiaSharp;
using ZXing;
using ZXing.SkiaSharp;

namespace SchoolExam.Infrastructure.QrCode;

public class ZXingNetQrCodeReader : IQrCodeReader
{
    private readonly BarcodeReader _reader;
    
    public ZXingNetQrCodeReader()
    {
        _reader = new BarcodeReader
        {
            Options =
            {
                TryHarder = true, PureBarcode = false, PossibleFormats = new List<BarcodeFormat>
                {
                    BarcodeFormat.QR_CODE
                }
            }
        };
    }
    public IEnumerable<string> ReadQrCodes(byte[] image)
    {
        using var bitmap = SKBitmap.Decode(image);
        var result = _reader.DecodeMultiple(bitmap);
        return result.Select(x => x.Text);
    }
}