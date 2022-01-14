using Microsoft.EntityFrameworkCore;
using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.Entities.SchoolAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;
using SchoolExam.Domain.Entities.UserAggregate;
using SchoolExam.Persistence.Base;
using SchoolExam.Persistence.Configuration;
using SchoolExam.Persistence.Configuration.CourseAggregate;
using SchoolExam.Persistence.Configuration.ExamAggregate;
using SchoolExam.Persistence.Configuration.PersonAggregate;
using SchoolExam.Persistence.Configuration.SchoolAggregate;
using SchoolExam.Persistence.Configuration.SubmissionAggregate;
using SchoolExam.Persistence.Configuration.UserAggregate;

namespace SchoolExam.Persistence.DataContext;

public class SchoolExamDbContext : DbContextBase
{
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
        modelBuilder.ApplyConfiguration(new ExamParticipantConfiguration());
        modelBuilder.ApplyConfiguration(new ExamStudentConfiguration());
        modelBuilder.ApplyConfiguration(new ExamCourseConfiguration());
        modelBuilder.ApplyConfiguration(new TaskPdfFileConfiguration());
        modelBuilder.ApplyConfiguration(new GradingTableConfiguration());
        modelBuilder.ApplyConfiguration(new ExamTaskConfiguration());
        modelBuilder.ApplyConfiguration(new BookletConfiguration());
        modelBuilder.ApplyConfiguration(new BookletPdfFileConfiguration());
        modelBuilder.ApplyConfiguration(new BookletPageConfiguration());

        modelBuilder.ApplyConfiguration(new SubmissionConfiguration());
        modelBuilder.ApplyConfiguration(new SubmissionPdfFileConfiguration());
        modelBuilder.ApplyConfiguration(new SubmissionPageConfiguration());
        modelBuilder.ApplyConfiguration(new SubmissionPagePdfFileConfiguration());
        modelBuilder.ApplyConfiguration(new AnswerConfiguration());
        modelBuilder.ApplyConfiguration(new AnswerSegmentConfiguration());

        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}