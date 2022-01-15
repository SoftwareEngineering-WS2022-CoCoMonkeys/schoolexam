using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.Application.Publishing;

public interface IPublishingService
{
    bool SendEmailToStudent(Booklet booklet, Exam exam);

    Task ScheduleSendEmailToStudent(IEnumerable<Booklet>  booklets, Exam exam, DateTime publishDateTime);

    Task DoPublishExam(IEnumerable<Booklet> booklets, Exam exam);
}