using SchoolExam.Domain.Base;

namespace SchoolExam.Domain.Entities.ExamAggregate;

public class ScheduledExam : EntityBase
{
    public Exam Exam { get; set; } = null!;
    public Guid ExamId { get; set; }
    public DateTime PublishTime { get; set; }
    public bool IsPublished { get; set; }
    
    protected ScheduledExam(Guid id) : base(id)
    {
    }
    
    public ScheduledExam(Guid id, Guid examId,  DateTime publishTime, bool isPublished) : this(id)
    {
        ExamId = examId;
        PublishTime = publishTime;
        IsPublished = isPublished;
    }
}