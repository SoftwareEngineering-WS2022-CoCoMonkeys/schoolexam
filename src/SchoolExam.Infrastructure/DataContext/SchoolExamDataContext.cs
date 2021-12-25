using Microsoft.EntityFrameworkCore;
using SchoolExam.Application.DataContext;
using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.Entities.SchoolAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;
using SchoolExam.Domain.Entities.UserAggregate;
using SchoolExam.Persistence.Base;
using SchoolExam.Persistence.DataContext;

namespace SchoolExam.Infrastructure.DataContext;

public class SchoolExamDataContext : DataContextBase<SchoolExamDbContext>, ISchoolExamDataContext
{
    public IQueryable<Course> Courses => Context.Courses.Include(Course.StudentsName).Include(Course.TeachersName);
    public IQueryable<Exam> Exams => Context.Exams.Include(x => x.GradingTable).Include(x => x.Tasks)
        .Include(x => x.Booklets).ThenInclude(x => x.Pages);
    public IQueryable<Student> Students =>
        Context.Students.Include(Student.CoursesName).Include(Student.LegalGuardiansName);
    public IQueryable<Teacher> Teachers =>
        Context.Teachers.Include(Teacher.CoursesName);
    public IQueryable<LegalGuardian> LegalGuardians => Context.LegalGuardians.Include(LegalGuardian.ChildrenName);
    public IQueryable<Submission> Submissions => Context.Submissions.Include(x => x.Pages).Include(x => x.Answers);
    public IQueryable<School> Schools => Context.Schools.Include(School.TeachersName);
    public IQueryable<User> Users => Context.Users;

    public SchoolExamDataContext(SchoolExamDbContext context) : base(context)
    {
    }
}