namespace SchoolExam.Application.Pdf;

public class RotationMatrix
{
    public float A { get; }
    public float B { get; }
    public float C { get; }
    public float D { get; }

    public RotationMatrix(float a, float b, float c, float d)
    {
        A = a;
        B = b;
        C = c;
        D = d;
    }

    public RotationMatrix(float theta) : this((float) Math.Cos(theta), -(float) Math.Sin(theta),
        (float) Math.Sin(theta), (float) Math.Cos(theta))
    {
        A = (float) Math.Cos(theta);
        B = -(float) Math.Sin(theta);
        C = (float) Math.Sin(theta);
        D = (float) Math.Cos(theta);
    }

    public float TransformX(float x, float y)
    {
        return A * x + B * y;
    }
    
    public float TransformY(float x, float y)
    {
        return C * x + D * y;
    }

    public static RotationMatrix operator *(RotationMatrix first, RotationMatrix second)
    {
        var a = first.A * second.A + first.B * second.C;
        var b = first.A * second.B + first.B * second.D;
        var c = first.C * second.A + first.D * second.C;
        var d = first.C * second.B + first.D * second.D;

        return new RotationMatrix(a, b, c, d);
    }
}