namespace SchoolExam.Application.TagLayout;

public class HermaMovables4203 : GapTagLayout<HermaMovables4203>
{
    public override int Rows => 6;
    public override int Columns => 3;
    public override float VerticalGap => PdfUnitConverter.ConvertMmToPoint(3.0f);
    public override float HorizontalGap => 0;
    public override TagSize TagSize => TagSize.FromMm(63.5f, 46.6f);
    public override PageSize PageSize => PageSize.A4;
    public override float Padding => PdfUnitConverter.ConvertMmToPoint(5);
}