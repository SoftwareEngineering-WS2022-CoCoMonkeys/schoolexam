using SchoolExam.Domain.Base;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Domain.Entities.ExamAggregate;

public class ExamTask : EntityBase
{
    public double MaxPoints { get; set; }
    public string Title { get; set; }
    public ExamPosition Start { get; set; }
    public ExamPosition End { get; set; }
    public Guid ExamId { get; set; }

    public ExamTask(Guid id) : base(id)
    {
    }

    public ExamTask(Guid id, string title, double maxPoints, Guid examId, ExamPosition start, ExamPosition end) :
        this(id)
    {
        Title = title;
        MaxPoints = maxPoints;
        ExamId = examId;
        Start = start;
        End = end;
    }
}