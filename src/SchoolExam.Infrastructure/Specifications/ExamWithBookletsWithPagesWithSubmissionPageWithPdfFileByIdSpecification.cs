using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class ExamWithBookletsWithPagesWithSubmissionPageWithPdfFileByIdSpecification : EntityByIdSpecification<Exam, Guid>
{
    public ExamWithBookletsWithPagesWithSubmissionPageWithPdfFileByIdSpecification(Guid id) : base(id)
    {
        AddInclude(x => x.Booklets);
        AddInclude($"{nameof(Exam.Booklets)}.{nameof(ExamBooklet.Pages)}");
        AddInclude($"{nameof(Exam.Booklets)}.{nameof(ExamBooklet.Pages)}.{nameof(ExamBookletPage.SubmissionPage)}");
        AddInclude(
            $"{nameof(Exam.Booklets)}.{nameof(ExamBooklet.Pages)}.{nameof(ExamBookletPage.SubmissionPage)}.{nameof(SubmissionPage.PdfFile)}");
    }
}