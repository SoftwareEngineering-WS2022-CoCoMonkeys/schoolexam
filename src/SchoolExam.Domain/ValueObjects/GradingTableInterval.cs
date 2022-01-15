namespace SchoolExam.Domain.ValueObjects;

public class GradingTableInterval
{
    public GradingTableIntervalBound Start { get; }
    public GradingTableIntervalBound End { get; }
    public string Grade { get; }

    public GradingTableInterval()
    {
    }

    public GradingTableInterval(GradingTableIntervalBound start, GradingTableIntervalBound end, string grade)
    {
        if (start.Type == GradingTableIntervalBoundType.Exclusive &&
            end.Type == GradingTableIntervalBoundType.Exclusive)
            throw new ArgumentException();
        Start = start;
        End = end;
        Grade = grade;
    }

    public bool Includes(int points)
    {
        bool greater = Start.Type == GradingTableIntervalBoundType.Exclusive
            ? points > Start.Points
            : points >= Start.Points;
        bool lower = End.Type == GradingTableIntervalBoundType.Exclusive
            ? points < End.Points
            : points <= End.Points;
        return greater && lower;
    }

    public bool Overlaps(GradingTableInterval other)
    {
        return Start.CompareTo(other.End) == -1 && other.Start.CompareTo(End) == -1;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;
        if (obj is not GradingTableInterval other)
            return false;
        return Equals(other);
    }

    protected bool Equals(GradingTableInterval other)
    {
        return Start.Equals(other.Start) && End.Equals(other.End) && Grade.Equals(other.Grade);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Start, End, Grade);
    }
}