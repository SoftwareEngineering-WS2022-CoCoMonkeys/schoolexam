namespace SchoolExam.Domain.ValueObjects;

public class Topic
{
    public string Name { get; }

    public Topic(string name)
    {
        Name = name;
    }

    public override bool Equals(object? obj)
    {
        return Name.Equals(obj);
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}