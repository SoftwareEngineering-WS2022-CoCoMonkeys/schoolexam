using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Application.Services;

public interface ISubmissionService
{
    Submission? GetById(Guid submissionId);
    Submission? GetByIdWithDetails(Guid submissionId);
    IEnumerable<Submission> GetByExam(Guid examId);
    byte[] GetSubmissionPdf(Guid submissionId);
}