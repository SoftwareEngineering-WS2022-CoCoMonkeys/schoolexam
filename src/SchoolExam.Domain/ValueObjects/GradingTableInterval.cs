using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Exceptions;

namespace SchoolExam.Domain.ValueObjects;

public class GradingTableInterval
{
    public GradingTableIntervalBound Start { get; set; }
    public GradingTableIntervalBound End { get; set; }
    public string Grade { get; set; }
    public GradingTableLowerBoundType Type { get; set; }
    public GradingTable GradingTable { get; set; }
    public Guid GradingTableId { get; set; }

    public GradingTableInterval()
    {
    }

    public GradingTableInterval(GradingTableIntervalBound start, GradingTableIntervalBound end, string grade,
        GradingTableLowerBoundType type, Guid gradingTableId)
    {
        if (start.Type == GradingTableIntervalBoundType.Exclusive &&
            end.Type == GradingTableIntervalBoundType.Exclusive)
            throw new DomainException("At least one interval bound must be inclusive.");
        Start = start;
        End = end;
        Grade = grade;
        Type = type;
        GradingTableId = gradingTableId;
    }

    public bool Includes(double points)
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