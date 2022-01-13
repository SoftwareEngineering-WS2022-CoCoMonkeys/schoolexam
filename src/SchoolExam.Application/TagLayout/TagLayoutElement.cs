namespace SchoolExam.Application.TagLayout;

public class TagLayoutElement
{
    public float Top { get; }
    public float Left { get; }
    public float Width { get; }
    public float Height { get; }

    public TagLayoutElement(float top, float left, float width, float height)
    {
        Top = top;
        Left = left;
        Width = width;
        Height = height;
    }
}