namespace SchoolExam.Application.Pdf;

public class PdfImageParseInfo
{
    public int Page { get; }
    public byte[] Data { get; }
    public RotationMatrix RotationMatrix { get; }

    public PdfImageParseInfo(int page, byte[] data, RotationMatrix rotationMatrix)
    {
        Page = page;
        Data = data;
        RotationMatrix = rotationMatrix;
    }
}