namespace SchoolExam.Application.Repositories;

public interface IExamRepository
{
    Task SetTaskPdfFile(Guid examId, string name, Guid uploaderId, byte[] content);
}