namespace SchoolExam.Domain.ValueObjects;

public class GradingTableIntervalBound : IComparable<GradingTableIntervalBound>
{
    public int Points { get; }
    public GradingTableIntervalBoundType Type { get; }

    public GradingTableIntervalBound(int points, GradingTableIntervalBoundType type)
    {
        Points = points;
        Type = type;
    }

    public int CompareTo(GradingTableIntervalBound? other)
    {
        if (other is null)
            throw new ArgumentException();
        if (Equals(other))
            return 0;
        if (Points == other.Points)
            return Type == GradingTableIntervalBoundType.Inclusive ? 1 : -1;
        return Points.CompareTo(other.Points);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;
        if (obj is not GradingTableIntervalBound other)
            return false;
        return Points == other.Points && Type == other.Type;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Points, Type);
    }
}