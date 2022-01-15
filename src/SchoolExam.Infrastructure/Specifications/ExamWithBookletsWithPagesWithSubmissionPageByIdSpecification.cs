using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class ExamWithBookletsWithPagesWithSubmissionPageByIdSpecification : EntityByIdSpecification<Exam>
{
    public ExamWithBookletsWithPagesWithSubmissionPageByIdSpecification(Guid id) : base(id)
    {
        AddInclude(x => x.Booklets);
        AddInclude($"{nameof(Exam.Booklets)}.{nameof(Booklet.Pages)}");
        AddInclude($"{nameof(Exam.Booklets)}.{nameof(Booklet.Pages)}.{nameof(BookletPage.SubmissionPage)}");
    }
}