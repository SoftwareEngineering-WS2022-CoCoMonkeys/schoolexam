using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.Entities.PersonAggregate;

namespace SchoolExam.Domain.Entities.ExamAggregate;

public class ExamCourse : ExamParticipant
{
    public Course Course { get; set; }
    public override ICollection<Student> Students => Course.Students.Select(x => x.Student).ToList();

    public ExamCourse(Guid examId, Guid participantId) : base(examId, participantId)
    {
    }
}