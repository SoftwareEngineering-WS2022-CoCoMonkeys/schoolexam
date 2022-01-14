using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class SubmissionPageByBookletPageSpecification : EntitySpecification<SubmissionPage>
{
    public SubmissionPageByBookletPageSpecification(Guid bookletPageId) : base(x =>
        x.BookletPageId.Equals(bookletPageId))
    {
    }
}