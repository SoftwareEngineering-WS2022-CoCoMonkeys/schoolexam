namespace SchoolExam.Application.Repositories;

public interface IExamRepository
{
    Task SetTaskPdfFile(Guid examId, string name, Guid userId, byte[] content);
    Task Build(Guid examId, int count, Guid userId);
    Task Match(Guid examId, byte[] pdf, Guid userId);
}