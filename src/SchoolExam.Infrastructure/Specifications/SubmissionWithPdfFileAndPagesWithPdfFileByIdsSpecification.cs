using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class SubmissionWithPdfFileAndPagesWithPdfFileByIdsSpecification : EntityByIdsSpecification<Submission, Guid>
{
    public SubmissionWithPdfFileAndPagesWithPdfFileByIdsSpecification(HashSet<Guid> ids) : base(ids)
    {
        AddInclude(x => x.PdfFile);
        AddInclude(x => x.Pages);
        AddInclude($"{nameof(Submission.Pages)}.{nameof(SubmissionPage.PdfFile)}");
    }
}