namespace SchoolExam.Application.TagLayout;

public interface ITagLayout<TLayout> where TLayout : ITagLayout<TLayout>, new()
{
    TagSize TagSize { get; }
    PageSize PageSize { get; }
    float Padding { get; }
    IEnumerable<TagLayoutElement> GetElements();
}