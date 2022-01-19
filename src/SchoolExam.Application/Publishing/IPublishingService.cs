namespace SchoolExam.Application.Publishing;

public interface IPublishingService
{
    Task ScheduleSendEmailToStudent(Guid examId, DateTime publishDateTime);
    Task DoPublishExam( Guid examId);
}