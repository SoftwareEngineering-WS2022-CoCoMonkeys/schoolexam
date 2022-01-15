using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.PersonAggregate;

namespace SchoolExam.Application.Publishing;

public interface IEmailCreator
{
    bool sendEmailToStudent(Student student, Exam exam);

    Task scheduleSendEmailToStudent(IEnumerable<Student>  student, Exam exam, DateTime publishDateTime);
}