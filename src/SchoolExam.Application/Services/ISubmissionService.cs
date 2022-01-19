using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Application.Services;

public interface ISubmissionService
{
    Submission? GetById(Guid submissionId);
    IEnumerable<Submission> GetByIds(IEnumerable<Guid> ids);
    Submission? GetByIdWithDetails(Guid submissionId);
    IEnumerable<Submission> GetByIdsWithDetails(IEnumerable<Guid> ids);
    IEnumerable<Submission> GetByExam(Guid examId);
}