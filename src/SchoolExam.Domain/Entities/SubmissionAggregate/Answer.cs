using SchoolExam.Domain.Base;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Domain.Entities.SubmissionAggregate;

public class Answer : EntityBase
{
    public AnswerState State { get; set; }
    public double? AchievedPoints { get; set; }
    public Guid TaskId { get; set; }
    public ExamTask Task { get; set; }
    public Guid SubmissionId { get; set; }
    public ICollection<AnswerSegment> Segments { get; set; }
    public DateTime UpdatedAt { get; set; }

    protected Answer(Guid id) : base(id)
    {
    }

    public Answer(Guid id, Guid taskId, Guid submissionId, AnswerState state, double? achievedPoints,
        DateTime updatedAt) : base(id)
    {
        TaskId = taskId;
        SubmissionId = submissionId;
        State = state;
        AchievedPoints = achievedPoints;
        UpdatedAt = updatedAt;
        Segments = new List<AnswerSegment>();
    }
}