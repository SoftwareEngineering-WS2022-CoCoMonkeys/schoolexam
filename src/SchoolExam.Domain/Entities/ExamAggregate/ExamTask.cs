using SchoolExam.Domain.Base;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Domain.Entities.ExamAggregate;

public class ExamTask : EntityBase<Guid>
{
    public double MaxPoints { get; set; }
    public string Title { get; set; }
    public int Number { get; set; }
    public ExamPosition Position { get; set; }

    public ExamTask(Guid id) : base(id)
    {
    }
    
    // TODO: numbering of tasks based on task position

    public ExamTask(Guid id, string title, double maxPoints, int number, ExamPosition position) : this(id)
    {
        Title = title;
        MaxPoints = maxPoints;
        Number = number;
        Position = position;
    }
}