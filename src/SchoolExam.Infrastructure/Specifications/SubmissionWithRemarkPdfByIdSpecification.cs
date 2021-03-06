using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class SubmissionWithRemarkPdfByIdSpecification : EntityByIdSpecification<Submission>
{
    public SubmissionWithRemarkPdfByIdSpecification(Guid id) : base(id)
    {
        AddInclude(x => x.RemarkPdfFile);
    }
}