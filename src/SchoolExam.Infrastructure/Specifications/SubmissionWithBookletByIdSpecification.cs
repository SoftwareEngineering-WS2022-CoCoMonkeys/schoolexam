using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class SubmissionWithBookletByIdSpecification : EntityByIdSpecification<Submission>
{
    public SubmissionWithBookletByIdSpecification(Guid id) : base(id)
    {
        AddInclude(x => x.Booklet);
    }
}