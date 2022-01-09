namespace SchoolExam.Application.Repositories;

public class ExamTaskInfo
{
    public Guid Id { get; }
    public string Title { get; }
    public double MaxPoints { get; }

    public ExamTaskInfo(Guid id, string title, double maxPoints)
    {
        Id = id;
        Title = title;
        MaxPoints = maxPoints;
    }
}