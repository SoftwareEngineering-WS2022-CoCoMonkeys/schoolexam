using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.PersonAggregate;

namespace SchoolExam.Application.Publishing;

public interface IPublishingService
{
    bool sendEmailToStudent(Booklet booklet, Exam exam);

    Task ScheduleSendEmailToStudent(IEnumerable<Booklet>  booklets, Exam exam, DateTime publishDateTime);

    Task DoPublishExam(IEnumerable<Booklet> booklets, Exam exam);
}