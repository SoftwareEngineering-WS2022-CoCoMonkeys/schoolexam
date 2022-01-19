namespace SchoolExam.Application.TagLayout;

public abstract class GapTagLayout<TLayout> : ITagLayout<TLayout> where TLayout : ITagLayout<TLayout>, new()
{
    public abstract int Rows { get; }
    public abstract int Columns { get; }
    public abstract float VerticalGap { get; }
    public abstract float HorizontalGap { get; }

    public abstract TagSize TagSize { get; }
    public abstract PageSize PageSize { get; }
    public abstract float Padding { get; }

    public IEnumerable<TagLayoutElement> GetElements()
    {
        var horizontalGaps = (Rows - 1) * HorizontalGap;
        var verticalGaps = (Columns - 1) * VerticalGap;
        var top = (PageSize.Height - TagSize.Height * Rows - horizontalGaps) / 2;
        var left = (PageSize.Width - TagSize.Width * Columns - verticalGaps) / 2;

        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                var tagTop = top + row * (TagSize.Height + HorizontalGap);
                var tagLeft = left + col * (TagSize.Width + VerticalGap);
                yield return new TagLayoutElement(tagTop, tagLeft, TagSize.Width, TagSize.Height);
            }
        }
    }
}