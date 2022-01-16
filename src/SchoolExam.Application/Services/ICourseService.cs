using SchoolExam.Domain.Entities.CourseAggregate;

namespace SchoolExam.Application.Services;

public interface ICourseService
{
    Course? GetById(Guid courseId);
    Task Create(Guid teacherId, string name, string topic);
    Task Update(Guid courseId, string name, string topic);
    Task Delete(Guid courseId);
    IEnumerable<Course> GetByTeacher(Guid teacherId);
}