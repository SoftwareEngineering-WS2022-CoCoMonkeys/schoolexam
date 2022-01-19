using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Application.Services;

public interface IMatchingService
{
    Task Match(Guid examId, byte[] pdf, Guid userId);
    IEnumerable<SubmissionPage> GetUnmatchedSubmissionPages(Guid examId);
    IEnumerable<BookletPage> GetUnmatchedBookletPages(Guid examId);
    Task MatchManually(Guid examId, Guid bookletPageId, Guid submissionPageId, Guid userId);
}