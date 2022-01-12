using SchoolExam.Domain.Entities.PersonAggregate;

namespace SchoolExam.Domain.Entities.ExamAggregate;

public class ExamStudent : ExamParticipant
{
    public Student Student { get; set; }
    public override ICollection<Student> Students => new List<Student> {Student};

    public ExamStudent(Guid examId, Guid participantId) : base(examId, participantId)
    {
    }
}