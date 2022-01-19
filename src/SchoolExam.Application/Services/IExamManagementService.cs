using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.Application.Services;

public interface IExamManagementService
{
    Exam? GetById(Guid examId);
    IEnumerable<Exam> GetByTeacher(Guid teacherId);
    Task<Guid> Create(string title, DateTime date, Guid teacherId, string topic);
    Task Update(Guid examId, string title, DateTime date);
    Task Delete(Guid examId);
    Task SetParticipants(Guid examId, IEnumerable<Guid> courseIds, IEnumerable<Guid> studentIds);
}