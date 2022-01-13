namespace SchoolExam.Application.TagLayout;

public class AveryZweckform3475200 : NoGapTagLayout<AveryZweckform3475200>
{
    public override TagSize TagSize => TagSize.FromMm(70, 36);
    public override int Rows => 8;
    public override int Columns => 3;
    public override PageSize PageSize => PageSize.A4;
    public override float Padding => PdfUnitConverter.ConvertMmToPoint(5);
}