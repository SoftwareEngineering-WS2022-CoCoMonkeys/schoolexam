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
    private readonly ISchoolExamRepository _repository;

    public CourseService(ISchoolExamRepository repository)
    {
        _repository = repository;
    }

    public Course? GetById(Guid id)
    {
        return _repository.Find(new CourseByIdSpecification(id));
    }

    public async Task Create(Guid teacherId, string name, string description, string topic)
    {
        var teacher = _repository.Find<Teacher, Guid>(teacherId);
        var course = new Course(Guid.NewGuid(), name, description, new Topic(topic), teacher.SchoolId);
        var courseTeacher = new CourseTeacher(course.Id, teacherId);
        _repository.Add(course);
        _repository.Add(courseTeacher);
        await _repository.SaveChangesAsync();
    }

    public async Task Update(Guid courseId, string name, string description, string topic)
    {
        var course = _repository.Find<Course, Guid>(courseId);
        if (course == null)
        {
            throw new ArgumentException("Course does not exist.");
        }
        course.Name = name;
        course.Description = description;
        course.Topic = new Topic(topic);
        await _repository.SaveChangesAsync();
    }

    public async Task Delete(Guid courseId)
    {
        var course = _repository.Find<Course, Guid>(courseId);
        if (course == null)
        {
            throw new ArgumentException("Course does not exist.");
        }

        _repository.Remove(course);
        await _repository.SaveChangesAsync();
    }

    public IEnumerable<Course> GetByTeacher(Guid teacherId)
    {
        var courseTeachers = _repository.List(new CourseTeacherByTeacherSpecification(teacherId));
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