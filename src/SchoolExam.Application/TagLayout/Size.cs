namespace SchoolExam.Application.TagLayout;

public class Size
{
    public float Width { get;}
    public float Height { get; }

    public Size(float width, float height)
    {
        if (width < 0 || height < 0)
        {
            throw new ArgumentException("Width and height of size must be positive.");
        }
        Width = width;
        Height = height;
    }
}