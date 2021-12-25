namespace SchoolExam.Domain.ValueObjects;

public class ExamTaskPosition
{
    public ExamPosition Start { get; }
    public ExamPosition End { get; }

    public ExamTaskPosition()
    {
    }

    public ExamTaskPosition(ExamPosition start, ExamPosition end)
    {
        if (start.CompareTo(end) > 0)
            throw new ArgumentException();
        Start = start;
        End = end;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;
        if (obj is not ExamTaskPosition other)
            return false;
        return Equals(other);
    }

    protected bool Equals(ExamTaskPosition other)
    {
        return Start.Equals(other.Start) && End.Equals(other.End);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Start, End);
    }
}