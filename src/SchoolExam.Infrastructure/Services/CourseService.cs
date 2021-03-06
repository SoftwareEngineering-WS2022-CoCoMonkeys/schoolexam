using SchoolExam.Application.Repository;
using SchoolExam.Application.Services;
using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.Exceptions;
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

    public async Task Create(Guid teacherId, string name, string? topic)
    {
        var teacher = _repository.Find<Teacher>(teacherId)!;
        var course = new Course(Guid.NewGuid(), name, topic != null ? new Topic(topic) : null, teacher.SchoolId);
        var courseTeacher = new CourseTeacher(course.Id, teacherId);
        _repository.Add(course);
        _repository.Add(courseTeacher);
        await _repository.SaveChangesAsync();
    }

    public async Task Update(Guid courseId, string name, string? topic)
    {
        var course = EnsureExamExists(courseId);
        course.Name = name;
        course.Topic = topic != null ? new Topic(topic) : null;
        await _repository.SaveChangesAsync();
    }

    public async Task AddStudents(Guid courseId, List<Guid> students)
    {
        var course = EnsureExamExists(courseId);
        foreach (var studentId in students)
        {
           var courseStudent = new CourseStudent(courseId, studentId);
           course.Students.Add(courseStudent); 
        }
        await _repository.SaveChangesAsync();
    }
    
    public async Task Delete(Guid courseId)
    {
        var course = _repository.Find<Course>(courseId);
        if (course == null)
        {
            throw new DomainException("Course does not exist.");
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
    
    private Course EnsureExamExists(Guid courseId)
    {
        var course = _repository.Find<Course>(courseId);
        if (course == null)
        {
            throw new DomainException("Course does not exist.");
        }

        return course;
    }
}