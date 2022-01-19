using SchoolExam.Application.Repository;
using SchoolExam.Application.Services;
using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.Exceptions;
using SchoolExam.Domain.Extensions;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Infrastructure.Extensions;
using SchoolExam.Infrastructure.Specifications;

namespace SchoolExam.Infrastructure.Services;

public class ExamManagementService : ExamServiceBase, IExamManagementService
{
    public ExamManagementService(ISchoolExamRepository repository) : base(repository)
    {
    }
    
    public Exam? GetById(Guid examId)
    {
        var result = Repository.Find<Exam>(examId);
        return result;
    }

    public IEnumerable<Exam> GetByTeacher(Guid teacherId)
    {
        var result = Repository.List(new ExamByTeacherSpecification(teacherId));
        return result;
    }
    
    public async Task<Guid> Create(string title, DateTime date, Guid teacherId, string topic)
    {
        var examId = Guid.NewGuid();
        var exam = new Exam(examId, title, date, teacherId, new Topic(topic));

        Repository.Add(exam);
        await Repository.SaveChangesAsync();

        return examId;
    }

    public async Task Update(Guid examId, string title, DateTime date)
    {
        var exam = EnsureExamExists(new EntityByIdSpecification<Exam>(examId));
        exam.Title = title;
        exam.Date = date;

        Repository.Update(exam);
        await Repository.SaveChangesAsync();
    }

    public async Task Delete(Guid examId)
    {
        var exam = EnsureExamExists(new EntityByIdSpecification<Exam>(examId));
        if (exam.State.HasBeenBuilt())
        {
            throw new DomainException("An exam that already has been built must not be deleted.");
        }

        Repository.Remove(exam);
        await Repository.SaveChangesAsync();
    }

    public async Task SetParticipants(Guid examId, IEnumerable<Guid> courseIds, IEnumerable<Guid> studentIds)
    {
        var exam = EnsureExamExists(new ExamWithParticipantsById(examId));
        
        if (exam.State.HasBeenBuilt())
        {
            throw new DomainException(
                "The participants of an exam that already has been built cannot be changed.");
        }

        var courseIdsSet = courseIds.ToHashSet();
        var examCourses = Repository.List<Course>(courseIdsSet);
        var studentIdsSet = studentIds.ToHashSet();
        var examStudents = Repository.List<Student>(studentIdsSet);

        if (examCourses.Count() != courseIdsSet.Count)
        {
            throw new DomainException("Course does not exist.");
        }
        
        if (examStudents.Count() != studentIdsSet.Count)
        {
            throw new DomainException("Student does not exist.");
        }
        
        // delete previously existing course participants
        foreach (var examCourse in exam.Participants.OfType<ExamCourse>())
        {
            Repository.Remove(examCourse);
        }

        foreach (var examStudent in exam.Participants.OfType<ExamStudent>())
        {
            Repository.Remove(examStudent);
        }

        // add participants
        foreach (var courseId in courseIdsSet)
        {
            var examCourse = new ExamCourse(examId, courseId);
            Repository.Add(examCourse);
        }

        foreach (var studentId in studentIdsSet)
        {
            var examStudent = new ExamStudent(examId, studentId);
            Repository.Add(examStudent);
        }

        await Repository.SaveChangesAsync();
    }
}