namespace SchoolExam.Application.Services;

public interface IExamTaskService
{
    Task SetTaskPdfFile(Guid examId, Guid userId, byte[] content, params ExamTaskInfo[] tasks);
}