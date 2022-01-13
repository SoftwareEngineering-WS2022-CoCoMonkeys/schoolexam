using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class SubmissionPdfFileBySubmissionSpecification : EntitySpecification<SubmissionPdfFile>
{
    public SubmissionPdfFileBySubmissionSpecification(Guid submissionId) : base(x => x.SubmissionId.Equals(submissionId))
    {
    }
}