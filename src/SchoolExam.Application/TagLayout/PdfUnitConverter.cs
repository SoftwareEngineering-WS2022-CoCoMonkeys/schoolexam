namespace SchoolExam.Application.TagLayout;

public static class PdfUnitConverter
{
    private const float InchToPoint = 72.0f;
    private const float InchToMm = 25.4f;

    public static float ConvertPointToInch(float point) => point / InchToPoint;
    public static float ConvertInchToPoint(float inch) => inch * InchToPoint;
    public static float ConvertPointToMm(float point) => point / InchToPoint * InchToMm;
    public static float ConvertMmToPoint(float mm) => mm / InchToMm * InchToPoint;
}