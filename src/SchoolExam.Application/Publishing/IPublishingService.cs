using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.Application.Publishing;

public interface IPublishingService
{
    bool SendEmailToStudent(Booklet booklet, Exam exam);

    Task ScheduleSendEmailToStudent(Guid examId, DateTime publishDateTime);

    Task DoPublishExam( Guid examId);
}