using SchoolExam.Application.DataContext;
using SchoolExam.Application.Repositories;
using SchoolExam.Domain.Entities.CourseAggregate;

namespace SchoolExam.Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly ISchoolExamDataContext _context;

    public CourseRepository(ISchoolExamDataContext context)
    {
        _context = context;
    }

    public Course? GetById(Guid id)
    {
        return _context.Courses.SingleOrDefault(x => x.Id.Equals(id));
    }

    public IEnumerable<Course> GetByTeacher(Guid teacherId)
    {
        return _context.Courses.Where(x => x.Teachers.Select(x => x.TeacherId).ToHashSet().Contains(teacherId))
            .AsEnumerable();
    }

    public IEnumerable<Course> GetByStudent(Guid studentId)
    {
        return _context.Courses.Where(x => x.Students.Select(x => x.StudentId).ToHashSet().Contains(studentId))
            .AsEnumerable();
    }
}