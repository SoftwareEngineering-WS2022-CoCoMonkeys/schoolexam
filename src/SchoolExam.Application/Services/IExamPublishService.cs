namespace SchoolExam.Application.Services;

public interface IExamPublishService
{
    Task Publish(Guid examId, DateTime? publishDateTime);
}