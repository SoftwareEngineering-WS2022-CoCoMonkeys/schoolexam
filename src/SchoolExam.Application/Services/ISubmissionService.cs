using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Application.Services;

public interface ISubmissionService
{
    Submission? GetById(Guid submissionId);
    IEnumerable<Submission> GetByIds(IEnumerable<Guid> ids);
    Submission? GetByIdWithDetails(Guid submissionId);
    IEnumerable<Submission> GetByIdsWithDetails(IEnumerable<Guid> ids);
    IEnumerable<Submission> GetByExam(Guid examId);
    byte[] GetSubmissionPdf(Guid submissionId);
    Task SetPoints(Guid submissionId, Guid taskId, double? points);
    Task SetRemark(Guid submissionId, byte[] remarkPdf, Guid userId);
}