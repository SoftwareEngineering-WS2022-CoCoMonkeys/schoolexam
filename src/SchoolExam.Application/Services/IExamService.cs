using SchoolExam.Application.TagLayout;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Application.Services;

public interface IExamService
{
    Exam? GetById(Guid examId);
    IEnumerable<Exam> GetByTeacher(Guid teacherId);
    IEnumerable<Exam> GetByStudent(Guid studentId);
    Task Create(string title, string description, DateTime date, Guid teacherId, string topic);
    Task Update(Guid examId, string title, string description, DateTime date);
    Task Delete(Guid examId);
    Task SetTaskPdfFile(Guid examId, Guid userId, byte[] content);
    Task FindTasks(Guid examId, Guid userId, params ExamTaskInfo[] tasks);
    Task<int> Build(Guid examId, Guid userId);
    byte[] GetParticipantQrCodePdf<TLayout>(Guid examId) where TLayout : ITagLayout<TLayout>, new();
    byte[] GetConcatenatedBookletPdfFile(Guid examId);
    Task Match(Guid examId, byte[] pdf, Guid userId);
    IEnumerable<SubmissionPage> GetUnmatchedSubmissionPages(Guid examId);
    IEnumerable<BookletPage> GetUnmatchedBookletPages(Guid examId);
    Task MatchManually(Guid examId, Guid bookletPageId, Guid submissionPageId, Guid userId);
    Task PublishExam(Guid examId, DateTime? publishDateTime);
    int GetMaxPoints(Guid examId);
    Task SetGradingTable(Guid examId, IEnumerable<GradingTableInterval> gradingTableIntervals);
}