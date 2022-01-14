namespace SchoolExam.Application.TagLayout;

public abstract class NoGapTagLayout<TLayout> : ITagLayout<TLayout> where TLayout : ITagLayout<TLayout>, new()
{
    public abstract int Rows { get; }
    public abstract int Columns { get; }

    public abstract TagSize TagSize { get; }
    public abstract PageSize PageSize { get; }
    public abstract float Padding { get; }

    public IEnumerable<TagLayoutElement> GetElements()
    {
        var top = (PageSize.Height - TagSize.Height * Rows) / 2;
        var left = (PageSize.Width - TagSize.Width * Columns) / 2;

        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                var tagTop = top + row * TagSize.Height;
                var tagLeft = left + col * TagSize.Width;
                yield return new TagLayoutElement(tagTop, tagLeft, TagSize.Width, TagSize.Height);
            }
        }
    }
}