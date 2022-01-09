using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class ExamWithBookletsWithPagesWithSubmissionPageByIdSpecification : EntityByIdSpecification<Exam, Guid>
{
    public ExamWithBookletsWithPagesWithSubmissionPageByIdSpecification(Guid id) : base(id)
    {
        AddInclude(x => x.Booklets);
        AddInclude($"{nameof(Exam.Booklets)}.{nameof(ExamBooklet.Pages)}");
        AddInclude($"{nameof(Exam.Booklets)}.{nameof(ExamBooklet.Pages)}.{nameof(ExamBookletPage.SubmissionPage)}");
    }
}