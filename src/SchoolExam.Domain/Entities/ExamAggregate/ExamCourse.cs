using SchoolExam.Domain.Entities.CourseAggregate;

namespace SchoolExam.Domain.Entities.ExamAggregate;

public class ExamCourse : ExamParticipant
{
    public Course Course { get; set; } = null!;

    public ExamCourse(Guid examId, Guid participantId) : base(examId, participantId)
    {
    }
}