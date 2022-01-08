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
    public IQueryable<Course> Courses => Context.Courses.Include(x => x.Students).Include(x => x.Teachers);

    public IQueryable<Exam> Exams => Context.Exams.Include(x => x.GradingTable).Include(x => x.TaskPdfFile)
        .ThenInclude(x => x.Uploader).Include(x => x.Tasks).Include(x => x.Booklets).ThenInclude(x => x.PdfFile)
        .ThenInclude(x => x.Uploader).Include(x => x.Booklets).ThenInclude(x => x.Pages)
        .ThenInclude(x => x.SubmissionPage);

    public IQueryable<ExamBooklet> ExamBooklets => Context.ExamBooklets;

    public IQueryable<ExamBookletPage> ExamBookletPages => Context.ExamBookletPages.Include(x => x.SubmissionPage);

    public IQueryable<Student> Students => Context.Students.Include(x => x.Courses).Include(x => x.LegalGuardians);

    public IQueryable<Teacher> Teachers => Context.Teachers.Include(x => x.Courses).ThenInclude(x => x.Course);

    public IQueryable<LegalGuardian> LegalGuardians => Context.LegalGuardians.Include(x => x.Children);

    public IQueryable<Submission> Submissions => Context.Submissions.Include(x => x.Pages).ThenInclude(x => x.PdfFile)
        .ThenInclude(x => x.Uploader).Include(x => x.Answers).Include(x => x.PdfFile).ThenInclude(x => x.Uploader);
    
    public IQueryable<SubmissionPage> SubmissionPages =>
        Context.SubmissionPages.Include(x => x.PdfFile).ThenInclude(x => x.Uploader);

    public IQueryable<School> Schools => Context.Schools.Include(x => x.Teachers);
    public IQueryable<User> Users => Context.Users;

    public SchoolExamDataContext(SchoolExamDbContext context) : base(context)
    {
    }
}