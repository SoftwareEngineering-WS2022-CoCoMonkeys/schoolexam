using SchoolExam.Domain.Entities.CourseAggregate;

namespace SchoolExam.Application.Repositories;

public interface ICourseRepository
{
    Course? GetById(Guid id);
    IEnumerable<Course> GetByTeacher(Guid teacherId);
    IEnumerable<Course> GetByStudent(Guid studentId);
}