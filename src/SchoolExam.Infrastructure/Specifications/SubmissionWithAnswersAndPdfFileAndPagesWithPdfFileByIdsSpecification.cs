using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class SubmissionWithAnswersAndPdfFileAndPagesWithPdfFileByIdsSpecification : EntityByIdsSpecification<Submission, Guid>
{
    public SubmissionWithAnswersAndPdfFileAndPagesWithPdfFileByIdsSpecification(HashSet<Guid> ids) : base(ids)
    {
        AddInclude(x => x.Answers);
        AddInclude(x => x.PdfFile);
        AddInclude(x => x.Pages);
        AddInclude($"{nameof(Submission.Pages)}.{nameof(SubmissionPage.PdfFile)}");
    }
}