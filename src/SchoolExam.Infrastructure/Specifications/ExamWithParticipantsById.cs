using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class ExamWithParticipantsById : EntityByIdSpecification<Exam>
{
    public ExamWithParticipantsById(Guid id) : base(id)
    {
        AddInclude(x => x.Participants);
        AddInclude($"{nameof(Exam.Participants)}.{nameof(ExamStudent.Student)}");
        AddInclude($"{nameof(Exam.Participants)}.{nameof(ExamCourse.Course)}");
        AddInclude($"{nameof(Exam.Participants)}.{nameof(ExamCourse.Course)}.{nameof(Course.Students)}");
        AddInclude(
            $"{nameof(Exam.Participants)}.{nameof(ExamCourse.Course)}.{nameof(Course.Students)}.{nameof(CourseStudent.Student)}");
    }
}