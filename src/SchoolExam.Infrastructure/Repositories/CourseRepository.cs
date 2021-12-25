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
        // use Contains method on IEnumerable since a course should not have to many teachers
        return _context.Courses.Where(x => x.TeacherIds.Contains(teacherId)).AsEnumerable();
    }

    public IEnumerable<Course> GetByStudent(Guid studentId)
    {
        return _context.Courses.Where(x => x.StudentIds.Contains(studentId)).AsEnumerable();
    }
}