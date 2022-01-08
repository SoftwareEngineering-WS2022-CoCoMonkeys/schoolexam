using SchoolExam.Domain.Base;

namespace SchoolExam.Domain.Entities.SubmissionAggregate;

public class Answer : EntityBase<Guid>
{
    public double AchievedPoints { get; set; }
    public Guid ExamTaskId { get; set; }
    public Guid SubmissionId { get; set; }

    protected Answer(Guid id) : base(id)
    {
    }

    public Answer(Guid id, double achievedPoints, Guid examTaskId, Guid submissionId) : base(id)
    {
        AchievedPoints = achievedPoints;
        ExamTaskId = examTaskId;
        SubmissionId = submissionId;
    }
}