using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class SubmissionWithAnswersByBookletSpecification : EntitySpecification<Submission>
{
    public SubmissionWithAnswersByBookletSpecification(Guid bookletId) : base(x => x.BookletId.Equals(bookletId))
    {
        AddInclude(x => x.Answers);
    }
}