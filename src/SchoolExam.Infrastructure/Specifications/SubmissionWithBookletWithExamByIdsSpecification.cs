using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class SubmissionWithBookletWithExamByIdsSpecification : EntityByIdsSpecification<Submission, Guid>
{
    public SubmissionWithBookletWithExamByIdsSpecification(HashSet<Guid> ids) : base(ids)
    {
        AddInclude(x => x.Booklet);
        AddInclude($"{nameof(Submission.Booklet)}.{nameof(Booklet.Exam)}");
    }
}