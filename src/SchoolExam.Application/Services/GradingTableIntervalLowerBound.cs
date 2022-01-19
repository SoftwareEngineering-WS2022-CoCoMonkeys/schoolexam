using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Application.Services;

public class GradingTableIntervalLowerBound
{
    public double Points { get; }
    public GradingTableLowerBoundType Type { get; }
    public string Grade { get; }

    public GradingTableIntervalLowerBound(double points, GradingTableLowerBoundType type, string grade)
    {
        Points = points;
        Type = type;
        Grade = grade;
    }
}