using SchoolExam.Domain.Entities.CourseAggregate;

namespace SchoolExam.Application.Services;

public interface ICourseService
{
    Course? GetById(Guid courseId);
    Task Create(Guid teacherId, string name, string description, string topic);
    Task Update(Guid courseId, string name, string description, string topic);
    Task Delete(Guid courseId);
    IEnumerable<Course> GetByTeacher(Guid teacherId);
    IEnumerable<Course> GetByStudent(Guid studentId);
}