namespace SchoolExam.Application.Services;

public class GradingTableIntervalLowerBound
{
    public double Points { get; set; }
    public string Grade { get; set; }

    public GradingTableIntervalLowerBound(double points, string grade)
    {
        Points = points;
        Grade = grade;
    }
}