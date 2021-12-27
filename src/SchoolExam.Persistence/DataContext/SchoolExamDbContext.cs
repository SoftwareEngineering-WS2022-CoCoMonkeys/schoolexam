using Microsoft.EntityFrameworkCore;
using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.Entities.SchoolAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;
using SchoolExam.Domain.Entities.UserAggregate;
using SchoolExam.Persistence.Base;
using SchoolExam.Persistence.Configuration;

namespace SchoolExam.Persistence.DataContext;

public class SchoolExamDbContext : DbContextBase
{
    public DbSet<Course> Courses { get; set; }
    public DbSet<Exam> Exams { get; set; }
    public DbSet<School> Schools { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<LegalGuardian> LegalGuardians { get; set; }
    public DbSet<Submission> Submissions { get; set; }
    public DbSet<User> Users { get; set; }

    public SchoolExamDbContext(IDbConnectionConfiguration configuration) : base(configuration)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new SchoolConfiguration());
        modelBuilder.ApplyConfiguration(new SchoolTeacherConfiguration());

        modelBuilder.ApplyConfiguration(new FileBaseConfiguration());

        modelBuilder.ApplyConfiguration(new PersonConfiguration());
        modelBuilder.ApplyConfiguration(new StudentConfiguration());
        modelBuilder.ApplyConfiguration(new StudentLegalGuardianConfiguration());
        modelBuilder.ApplyConfiguration(new TeacherConfiguration());
        modelBuilder.ApplyConfiguration(new LegalGuardianConfiguration());

        modelBuilder.ApplyConfiguration(new CourseConfiguration());
        modelBuilder.ApplyConfiguration(new CourseTeacherConfiguration());
        modelBuilder.ApplyConfiguration(new CourseStudentConfiguration());

        modelBuilder.ApplyConfiguration(new ExamConfiguration());
        modelBuilder.ApplyConfiguration(new TaskPdfFileConfiguration());
        modelBuilder.ApplyConfiguration(new GradingTableConfiguration());
        modelBuilder.ApplyConfiguration(new ExamTaskConfiguration());
        modelBuilder.ApplyConfiguration(new ExamBookletConfiguration());
        modelBuilder.ApplyConfiguration(new ExamBookletPageConfiguration());

        modelBuilder.ApplyConfiguration(new SubmissionConfiguration());
        modelBuilder.ApplyConfiguration(new SubmissionPageConfiguration());
        modelBuilder.ApplyConfiguration(new AnswerConfiguration());

        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}