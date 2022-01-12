namespace SchoolExam.Domain.Entities.ExamAggregate;

public abstract class ExamParticipant
{
    public Guid ExamId { get; set; }
    public Guid ParticipantId { get; set; }

    public ExamParticipant(Guid examId, Guid participantId)
    {
        ExamId = examId;
        ParticipantId = participantId;
    }
}