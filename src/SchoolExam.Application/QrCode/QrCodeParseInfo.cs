namespace SchoolExam.Application.QrCode;

public class QrCodeParseInfo
{
    public QrCodeAlignmentPatternPosition Orientation { get; }
    public string Data { get; }
    public int Degrees => GetDegrees();

    public QrCodeParseInfo(QrCodeAlignmentPatternPosition orientation, string data)
    {
        Orientation = orientation;
        Data = data;
    }

    private int GetDegrees()
    {
        switch (Orientation)
        {
            case QrCodeAlignmentPatternPosition.BottomRight: return 0;
            case QrCodeAlignmentPatternPosition.BottomLeft: return 90;
            case QrCodeAlignmentPatternPosition.TopLeft: return 180;
            case QrCodeAlignmentPatternPosition.TopRight: return 270;
            default: return 0;
        }
    }
}