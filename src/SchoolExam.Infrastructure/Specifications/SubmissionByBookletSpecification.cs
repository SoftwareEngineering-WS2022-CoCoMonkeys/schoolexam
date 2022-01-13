using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class SubmissionByBookletSpecification : EntitySpecification<Submission>
{
    public SubmissionByBookletSpecification(Guid bookletId) : base(x => x.BookletId.Equals(bookletId))
    {
    }
}