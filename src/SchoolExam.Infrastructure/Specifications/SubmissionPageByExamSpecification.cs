using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class SubmissionPageByExamSpecification : EntitySpecification<SubmissionPage>
{
    public SubmissionPageByExamSpecification(Guid examId) : base(x => x.ExamId.Equals(examId))
    {
    }
}