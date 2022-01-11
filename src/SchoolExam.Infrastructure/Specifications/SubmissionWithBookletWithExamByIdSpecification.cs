using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class SubmissionWithBookletWithExamByIdSpecification : EntityByIdSpecification<Submission, Guid>
{
    public SubmissionWithBookletWithExamByIdSpecification(Guid id) : base(id)
    {
        AddInclude(x => x.Booklet);
        AddInclude($"{nameof(Submission.Booklet)}.{nameof(ExamBooklet.Exam)}");
    }
}