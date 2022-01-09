using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Application.Repositories;

public interface IExamRepository
{
    IEnumerable<Exam> GetByTeacher(Guid teacherId);
    IEnumerable<Exam> GetByStudent(Guid studentId);
    Task Create(string title, string description, DateTime date, Guid courseId, Guid teacherId);
    Task Update(Guid examId, string title, string description, DateTime date);
    Task Delete(Guid examId);
    Task SetTaskPdfFile(Guid examId, Guid userId, byte[] content);
    Task FindTasks(Guid examId, Guid userId, params ExamTaskInfo[] tasks);
    Task Build(Guid examId, int count, Guid userId);
    Task Clean(Guid examId);
    Task Match(Guid examId, byte[] pdf, Guid userId);
    IEnumerable<SubmissionPage> GetUnmatchedSubmissionPages(Guid examId);
    IEnumerable<ExamBookletPage> GetUnmatchedBookletPages(Guid examId);
    Task MatchManually(Guid examId, Guid bookletPageId, Guid submissionPageId, Guid userId);
}