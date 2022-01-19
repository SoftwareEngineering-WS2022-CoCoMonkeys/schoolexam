using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.Entities.PersonAggregate;

namespace SchoolExam.Application.Services;

public interface ICourseService
{
    Course? GetById(Guid courseId);
    Task Create(Guid teacherId, string name, string? topic);
    Task Update(Guid courseId, string name, string? topic);

    Task AddStudents(Guid courseId, List<Guid> students);
    
    Task Delete(Guid courseId);
    IEnumerable<Course> GetByTeacher(Guid teacherId);
    
    
}