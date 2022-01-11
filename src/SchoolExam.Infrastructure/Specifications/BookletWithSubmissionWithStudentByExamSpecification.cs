using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class BookletWithSubmissionWithStudentByExamSpecification : EntitySpecification<ExamBooklet>
{
    public BookletWithSubmissionWithStudentByExamSpecification(Guid examId) : base(x =>
        x.ExamId.Equals(examId) && x.Submission != null)
    {
        AddInclude(x => x.Submission);
        AddInclude($"{nameof(ExamBooklet.Submission)}.{nameof(Submission.Student)}");
    }
}