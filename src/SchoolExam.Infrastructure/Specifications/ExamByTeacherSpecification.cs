using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class ExamByTeacherSpecification : EntitySpecification<Exam>
{
    public ExamByTeacherSpecification(Guid teacherId) : base(x => x.CreatorId.Equals(teacherId))
    {
        AddInclude(x => x.Participants);
        AddInclude($"{nameof(Exam.Participants)}.{nameof(ExamCourse.Course)}");
        AddInclude($"{nameof(Exam.Participants)}.{nameof(ExamCourse.Course)}.{nameof(Course.Students)}");
        AddInclude(
            $"{nameof(Exam.Participants)}.{nameof(ExamCourse.Course)}.{nameof(Course.Students)}.{nameof(CourseStudent.Student)}");
        AddInclude(x => x.Booklets);
        AddInclude($"{nameof(Exam.Booklets)}.{nameof(Booklet.Submission)}");
        AddInclude($"{nameof(Exam.Booklets)}.{nameof(Booklet.Submission)}.{nameof(Submission.Answers)}");
        AddInclude(x => x.Tasks);
    }
}