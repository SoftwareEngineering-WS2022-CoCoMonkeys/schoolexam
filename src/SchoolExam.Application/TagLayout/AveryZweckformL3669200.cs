namespace SchoolExam.Application.TagLayout;

public class AveryZweckformL3669200 : NoGapTagLayout<AveryZweckformL3669200>
{
    public override int Rows => 5;
    public override int Columns => 3;
    public override TagSize TagSize => TagSize.FromMm(70.0f, 50.8f);
    public override PageSize PageSize => PageSize.A4;
    public override float Padding => PdfUnitConverter.ConvertMmToPoint(5);
}