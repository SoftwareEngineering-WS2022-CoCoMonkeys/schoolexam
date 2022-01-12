using System.ComponentModel.DataAnnotations.Schema;
using SchoolExam.Domain.Entities.PersonAggregate;

namespace SchoolExam.Domain.Entities.ExamAggregate;

public abstract class ExamParticipant
{
    public Guid ExamId { get; set; }
    public Guid ParticipantId { get; set; }
    [NotMapped]
    public abstract ICollection<Student> Students { get; }

    public ExamParticipant(Guid examId, Guid participantId)
    {
        ExamId = examId;
        ParticipantId = participantId;
    }
}