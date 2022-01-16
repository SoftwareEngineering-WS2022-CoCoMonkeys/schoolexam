using SchoolExam.Application.TagLayout;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Application.Services;

public interface IExamService
{
    Exam? GetById(Guid examId);
    IEnumerable<Exam> GetByTeacher(Guid teacherId);
    Task Create(string title, DateTime date, Guid teacherId, string topic);
    Task Update(Guid examId, string title, DateTime date);
    Task Delete(Guid examId);
    Task SetParticipants(Guid examId, IEnumerable<Guid> courseIds, IEnumerable<Guid> studentIds);
    Task SetTaskPdfFile(Guid examId, Guid userId, byte[] content, params ExamTaskInfo[] tasks);
    Task<int> Build(Guid examId, Guid userId);
    Task Clean(Guid examId);
    byte[] GetParticipantQrCodePdf<TLayout>(Guid examId) where TLayout : ITagLayout<TLayout>, new();
    byte[] GetConcatenatedBookletPdfFile(Guid examId);
    Task Match(Guid examId, byte[] pdf, Guid userId);
    IEnumerable<SubmissionPage> GetUnmatchedSubmissionPages(Guid examId);
    IEnumerable<BookletPage> GetUnmatchedBookletPages(Guid examId);
    Task MatchManually(Guid examId, Guid bookletPageId, Guid submissionPageId, Guid userId);
    Task Publish(Guid examId, DateTime? publishDateTime);
    double GetMaxPoints(Guid examId);
    Task SetGradingTable(Guid examId, params GradingTableIntervalLowerBound[] lowerBounds);
}