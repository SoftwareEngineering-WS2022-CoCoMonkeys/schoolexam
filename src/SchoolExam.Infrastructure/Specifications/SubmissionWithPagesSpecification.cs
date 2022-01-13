using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class SubmissionWithPagesSpecification : EntitySpecification<Submission>
{
    public SubmissionWithPagesSpecification()
    {
        AddInclude(x => x.Pages);
    }
}