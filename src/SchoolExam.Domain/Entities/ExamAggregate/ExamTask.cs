using SchoolExam.Domain.Base;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Domain.Entities.ExamAggregate;

public class ExamTask : EntityBase<Guid>
{
    public int MaxPoints { get; set; }
    public int Number { get; set; }
    public ExamTaskPosition Position { get; set; }

    public ExamTask(Guid id) : base(id)
    {
    }

    public ExamTask(Guid id, int maxPoints, int number, ExamTaskPosition position) : this(id)
    {
        MaxPoints = maxPoints;
        Number = number;
        Position = position;
    }
}