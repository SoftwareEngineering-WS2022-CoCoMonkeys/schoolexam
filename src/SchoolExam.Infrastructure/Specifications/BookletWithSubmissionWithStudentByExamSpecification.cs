using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class BookletWithSubmissionWithStudentByExamSpecification : EntitySpecification<Booklet>
{
    public BookletWithSubmissionWithStudentByExamSpecification(Guid examId) : base(x =>
        x.ExamId.Equals(examId) && x.Submission != null)
    {
        AddInclude(x => x.Submission);
        AddInclude($"{nameof(Booklet.Submission)}.{nameof(Submission.Student)}");
    }
}