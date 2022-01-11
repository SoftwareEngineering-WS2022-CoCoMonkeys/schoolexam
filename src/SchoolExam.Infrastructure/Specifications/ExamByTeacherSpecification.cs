using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class ExamByTeacherSpecification : EntitySpecification<Exam>
{
    public ExamByTeacherSpecification(Guid teacherId) : base(x => x.CreatorId.Equals(teacherId))
    {
        AddInclude(x => x.Course);
        AddInclude($"{nameof(Exam.Course)}.{nameof(Course.Students)}");
        AddInclude(x => x.Booklets);
        AddInclude($"{nameof(Exam.Booklets)}.{nameof(Booklet.Submission)}");
        AddInclude($"{nameof(Exam.Booklets)}.{nameof(Booklet.Submission)}.{nameof(Submission.Answers)}");
        AddInclude(x => x.Tasks);
    }
}