using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class ExamWithTaskPdfFileAndParticipantsById : EntityByIdSpecification<Exam, Guid>
{
    public ExamWithTaskPdfFileAndParticipantsById(Guid id) : base(id)
    {
        AddInclude(x => x.TaskPdfFile);
        AddInclude(x => x.Participants);
        AddInclude($"{nameof(Exam.Participants)}.{nameof(ExamCourse.Course)}");
        AddInclude($"{nameof(Exam.Participants)}.{nameof(ExamCourse.Course)}.{nameof(Course.Students)}");
    }
}