using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class ExamWithBookletsWithPagesWithSubmissionPageWithPdfFileByIdSpecification : EntityByIdSpecification<Exam>
{
    public ExamWithBookletsWithPagesWithSubmissionPageWithPdfFileByIdSpecification(Guid id) : base(id)
    {
        AddInclude(x => x.Booklets);
        AddInclude($"{nameof(Exam.Booklets)}.{nameof(Booklet.Pages)}");
        AddInclude($"{nameof(Exam.Booklets)}.{nameof(Booklet.Pages)}.{nameof(BookletPage.SubmissionPage)}");
        AddInclude(
            $"{nameof(Exam.Booklets)}.{nameof(Booklet.Pages)}.{nameof(BookletPage.SubmissionPage)}.{nameof(SubmissionPage.PdfFile)}");
    }
}