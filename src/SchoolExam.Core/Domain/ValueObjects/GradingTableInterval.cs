using System;

namespace SchoolExam.Core.Domain.ValueObjects
{
    public class GradingTableInterval
    {
        public GradingTableIntervalBound Start { get; }
        public GradingTableIntervalBound End { get; }
        public double Grade { get; }

        public GradingTableInterval()
        {
        }

        public GradingTableInterval(GradingTableIntervalBound start, GradingTableIntervalBound end, double grade)
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
    }
}