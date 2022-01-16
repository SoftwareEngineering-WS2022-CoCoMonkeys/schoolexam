using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class RemarkPdfFileBySubmissionSpecification : EntitySpecification<RemarkPdfFile>
{
    public RemarkPdfFileBySubmissionSpecification(Guid submissionId) : base(x => x.SubmissionId.Equals(submissionId))
    {
    }
}