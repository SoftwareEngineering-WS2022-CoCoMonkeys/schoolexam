using SchoolExam.Application.Repository;
using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.Exceptions;
using SchoolExam.Infrastructure.Specifications;

namespace SchoolExam.Infrastructure.Services;

public abstract class ExamServiceBase
{
    protected ISchoolExamRepository Repository { get; }
    
    protected ExamServiceBase(ISchoolExamRepository repository)
    {
        Repository = repository;
    }
    
    protected Exam EnsureExamExists(EntityByIdSpecification<Exam> spec)
    {
        var exam = Repository.Find(spec);
        if (exam == null)
        {
            throw new DomainException("Exam does not exist.");
        }

        return exam;
    }
    
    protected IEnumerable<Student> GetStudentsByExam(Guid examId)
    {
        var examWithStudents = Repository.Find(new ExamWithParticipantsById(examId))!;
        var students = examWithStudents.Participants.OfType<ExamStudent>().Select(x => x.Student)
            .Union(examWithStudents.Participants.OfType<ExamCourse>()
                .SelectMany(x => x.Course.Students.Select(s => s.Student)));

        return students;
    }
}