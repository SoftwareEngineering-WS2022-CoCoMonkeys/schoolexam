using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class ExamWithTasksAndBookletsWithPagesWithSubmissionPageByIdSpecification : EntityByIdSpecification<Exam, Guid>
{
    public ExamWithTasksAndBookletsWithPagesWithSubmissionPageByIdSpecification(Guid id) : base(id)
    {
        AddInclude(x => x.Tasks);
        AddInclude(x => x.Booklets);
        AddInclude($"{nameof(Exam.Booklets)}.{nameof(ExamBooklet.Pages)}");
        AddInclude($"{nameof(Exam.Booklets)}.{nameof(ExamBooklet.Pages)}.{nameof(ExamBookletPage.SubmissionPage)}");
    }
}