using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.Entities.SchoolAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;
using SchoolExam.Domain.Entities.UserAggregate;

namespace SchoolExam.Application.DataContext;

public interface ISchoolExamDataContext : IDataContext
{
    IQueryable<Course> Courses { get; }
    IQueryable<Exam> Exams { get; }
    IQueryable<Student> Students { get; }
    IQueryable<Teacher> Teachers { get; }
    IQueryable<LegalGuardian> LegalGuardians { get; }
    IQueryable<Submission> Submissions { get; }
    IQueryable<School> Schools { get; }
    IQueryable<User> Users { get; }
}