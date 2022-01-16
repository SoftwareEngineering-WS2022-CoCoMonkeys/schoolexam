using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class SubmissionWithAnswersByExamSpecification : EntitySpecification<Submission>
{
    public SubmissionWithAnswersByExamSpecification(Guid examId) : base(x => x.Booklet.ExamId.Equals(examId))
    {
        AddInclude(x => x.Booklet);
    }
}