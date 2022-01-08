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
        var teacher = _context.Teachers.Single(x => x.Id.Equals(teacherId));
        return teacher.Courses.Select(x => x.Course);
    }

    public IEnumerable<Course> GetByStudent(Guid studentId)
    {
        // return _context.Courses.Where(x => x.Students.Select( => x.StudentId).ToHashSet().Contains(studentId))
        //     .AsEnumerable();
        throw new NotImplementedException();
    }
}