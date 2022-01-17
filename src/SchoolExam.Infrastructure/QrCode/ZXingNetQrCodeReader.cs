using SchoolExam.Application.Pdf;
using SchoolExam.Application.QrCode;
using SkiaSharp;
using ZXing;
using ZXing.QrCode.Internal;
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

    public IEnumerable<QrCodeParseInfo> ReadQrCodes(byte[] image, RotationMatrix rotationMatrix)
    {
        using var bitmap = SKBitmap.Decode(image);
        var decoded = _reader.DecodeMultiple(bitmap);
        var result = decoded?.Where(IsValidQrCode) ?? Enumerable.Empty<Result>();
        return result.Select(x => new QrCodeParseInfo(GetOrientation(x, rotationMatrix), x.Text));
    }

    private bool IsValidQrCode(Result result)
    {
        return result.ResultPoints.Count(x => x is FinderPattern) == 3 &&
               result.ResultPoints.Count(x => x is AlignmentPattern) == 1;
    }

    private QrCodeAlignmentPatternPosition GetOrientation(Result result, RotationMatrix rotationMatrix)
    {
        var points = result.ResultPoints.OrderBy(x => rotationMatrix.TransformX(x.X, x.Y))
            .ThenBy(x => rotationMatrix.TransformY(x.X, x.Y)).ToArray();
        var position = points.TakeWhile(x => x is not AlignmentPattern).Count();
        switch (position)
        {
            case 0:
            case 1:
                return position == 0 ^ rotationMatrix.TransformY(points[0].X, points[0].Y) >
                    rotationMatrix.TransformY(points[1].X, points[1].Y)
                        ? QrCodeAlignmentPatternPosition.TopLeft
                        : QrCodeAlignmentPatternPosition.BottomLeft;
            case 2:
            case 3:
                return position == 2 ^ rotationMatrix.TransformY(points[2].X, points[2].Y) >
                    rotationMatrix.TransformY(points[3].X, points[3].Y)
                        ? QrCodeAlignmentPatternPosition.TopRight
                        : QrCodeAlignmentPatternPosition.BottomRight;
            default:
                throw new InvalidOperationException(
                    "QR code must have exactly three finder patterns and one alignment pattern to find out the orientation");
        }
    }
}