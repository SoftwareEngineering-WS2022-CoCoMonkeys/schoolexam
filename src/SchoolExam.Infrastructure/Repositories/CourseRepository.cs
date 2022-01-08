using SchoolExam.Application.DataContext;
using SchoolExam.Application.Repositories;
using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.ValueObjects;

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

    public async Task Create(Guid teacherId, string name, string description, string subject)
    {
        var teacher = _context.Teachers.Single(x => x.Id.Equals(teacherId));
        var course = new Course(Guid.NewGuid(), name, description, new Subject(subject), teacher.SchoolId);
        var courseTeacher = new CourseTeacher(course.Id, teacherId);
        _context.Add(course);
        _context.Add(courseTeacher);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Guid courseId, string name, string description, string subject)
    {
        var course = _context.Courses.SingleOrDefault(x => x.Id.Equals(courseId));
        if (course == null)
        {
            throw new ArgumentException("Course does not exist.");
        }
        course.Name = name;
        course.Description = description;
        course.Subject = new Subject(subject);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Guid courseId)
    {
        var course = _context.Courses.SingleOrDefault(x => x.Id.Equals(courseId));
        if (course == null)
        {
            throw new ArgumentException("Course does not exist.");
        }

        _context.Remove(course);
        await _context.SaveChangesAsync();
    }

    public IEnumerable<Course> GetByTeacher(Guid teacherId)
    {
        var teacher = _context.Teachers.Single(x => x.Id.Equals(teacherId));
        return teacher.Courses.Select(x => x.Course);
    }

    public IEnumerable<Course> GetByStudent(Guid studentId)
    {
        // TODO
        // return _context.Courses.Where(x => x.Students.Select( => x.StudentId).ToHashSet().Contains(studentId))
        //     .AsEnumerable();
        throw new NotImplementedException();
    }
}