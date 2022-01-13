using SchoolExam.Application.Repository;
using SchoolExam.Application.Services;
using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Infrastructure.Extensions;
using SchoolExam.Infrastructure.Specifications;

namespace SchoolExam.Infrastructure.Services;

public class CourseService : ICourseService
{
    private readonly ISchoolExamRepository _context;

    public CourseService(ISchoolExamRepository context)
    {
        _context = context;
    }

    public Course? GetById(Guid id)
    {
        return _context.Find(new CourseByIdSpecification(id));
    }

    public async Task Create(Guid teacherId, string name, string description, string subject)
    {
        var teacher = _context.Find<Teacher, Guid>(teacherId);
        var course = new Course(Guid.NewGuid(), name, description, new Subject(subject), teacher.SchoolId);
        var courseTeacher = new CourseTeacher(course.Id, teacherId);
        _context.Add(course);
        _context.Add(courseTeacher);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Guid courseId, string name, string description, string subject)
    {
        var course = _context.Find<Course, Guid>(courseId);
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
        var course = _context.Find<Course, Guid>(courseId);
        if (course == null)
        {
            throw new ArgumentException("Course does not exist.");
        }

        _context.Remove(course);
        await _context.SaveChangesAsync();
    }

    public IEnumerable<Course> GetByTeacher(Guid teacherId)
    {
        var courseTeachers = _context.List(new CourseTeacherByTeacherSpecification(teacherId));
        var result = courseTeachers.Select(x => x.Course);
        return result;
    }

    public IEnumerable<Course> GetByStudent(Guid studentId)
    {
        // TODO
        // return _context.Courses.Where(x => x.Students.Select( => x.StudentId).ToHashSet().Contains(studentId))
        //     .AsEnumerable();
        throw new NotImplementedException();
    }
}