using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Common.Infrastructure.EFAbstractions;
using Microsoft.EntityFrameworkCore;
using SchoolExam.Core.Domain.CourseAggregate;
using SchoolExam.Core.Domain.ExamAggregate;
using SchoolExam.Core.Domain.PersonAggregate;
using SchoolExam.Core.Domain.SchoolAggregate;
using SchoolExam.Core.Domain.SubmissionAggregate;
using Task = SchoolExam.Core.Domain.TaskAggregate.Task;
using SchoolExam.Core.Domain.ValueObjects;

namespace SchoolExam.Infrastructure.DataContext
{
    public class SchoolExamDbContext : DbContextBase
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<LegalGuardian> LegalGuardians { get; set; }
        public DbSet<Task> Tasks { get; set; }

        public SchoolExamDbContext(IDbConnectionConfiguration configuration) : base(configuration)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<School>().ToTable("School");
            modelBuilder.Entity<School>().HasKey(x => x.Id);
            OwnsAddress<School>(modelBuilder, x => x.Location);

            modelBuilder.Entity<Person>().ToTable("Person");
            modelBuilder.Entity<Person>().HasDiscriminator();
            modelBuilder.Entity<Person>().HasKey(x => x.Id);
            OwnsAddress<Person>(modelBuilder, x => x.Address);

            modelBuilder.Entity<Student>().HasMany<LegalGuardian>(Student.LegalGuardiansName)
                .WithMany(LegalGuardian.ChildrenName).UsingEntity<Dictionary<string, object>>(
                    "StudentLegalGuardian",
                    x => x.HasOne<LegalGuardian>().WithMany().HasForeignKey("LegalGuardianId"),
                    x => x.HasOne<Student>().WithMany().HasForeignKey("StudentId"));

            modelBuilder.Entity<Teacher>().HasMany<School>(Teacher.SchoolsName).WithMany(School.TeachersName)
                .UsingEntity<Dictionary<string, object>>(
                    "SchoolTeacher",
                    x => x.HasOne<School>().WithMany().HasForeignKey("SchoolId"),
                    x => x.HasOne<Teacher>().WithMany().HasForeignKey("TeacherId"));

            modelBuilder.Entity<Course>().ToTable("Course");
            modelBuilder.Entity<Course>().HasKey(x => x.Id);
            modelBuilder.Entity<Course>().OwnsOne(x => x.Subject,
                x => { x.Property(s => s.Name).HasColumnName("Subject").IsRequired(false); });
            modelBuilder.Entity<Course>().Property(x => x.Year).HasDefaultValue(DateTime.Now.Year);
            modelBuilder.Entity<Course>().HasMany<Exam>();
            modelBuilder.Entity<Course>().HasOne<Teacher>().WithMany(Teacher.CoursesName);
            modelBuilder.Entity<Course>().HasMany<Student>(Course.StudentsName).WithMany(Student.CoursesName)
                .UsingEntity<Dictionary<string, object>>(
                    "CourseStudent",
                    x => x.HasOne<Student>().WithMany().HasForeignKey("StudentId"),
                    x => x.HasOne<Course>().WithMany().HasForeignKey("CourseId")
                );

            modelBuilder.Entity<GradingTable>().ToTable("GradingTable");
            modelBuilder.Entity<GradingTable>().HasKey(x => x.Id);
            modelBuilder.Entity<GradingTable>().OwnsMany(x => x.Intervals, x =>
            {
                x.WithOwner().HasForeignKey("GradingTableId");
                x.OwnsOne(y => y.Start, y =>
                {
                    y.Property(z => z.Points).HasColumnName("StartPoints");
                    y.Property(z => z.Type).HasColumnName("StartType");
                });
                x.OwnsOne(y => y.End, y =>
                {
                    y.Property(z => z.Points).HasColumnName("EndPoints");
                    y.Property(z => z.Type).HasColumnName("EndType");
                });
                x.Property(y => y.Grade).HasColumnName("Grade");
            });

            modelBuilder.Entity<Exam>().ToTable("Exam");
            modelBuilder.Entity<Exam>().HasKey(x => x.Id);
            modelBuilder.Entity<Exam>().HasOne(x => x.GradingTable);
            modelBuilder.Entity<Exam>().HasMany(x => x.Tasks);
            modelBuilder.Entity<Exam>().HasMany(x => x.Booklets);

            modelBuilder.Entity<ExamTask>().ToTable("ExamTask");
            modelBuilder.Entity<ExamTask>().HasKey(x => x.Id);
            modelBuilder.Entity<ExamTask>().OwnsOne(x => x.Position, x =>
            {
                x.OwnsOne(y => y.Start, y =>
                {
                    y.Property(z => z.Page).HasColumnName("StartPage");
                    y.Property(z => z.Y).HasColumnName("StartY");
                });
                x.OwnsOne(y => y.End, y =>
                {
                    y.Property(z => z.Page).HasColumnName("EndPage");
                    y.Property(z => z.Y).HasColumnName("EndY");
                });
            });
            
            modelBuilder.Entity<ExamBooklet>().ToTable("ExamBooklet");
            modelBuilder.Entity<ExamBooklet>().HasKey(x => x.Id);
            modelBuilder.Entity<ExamBooklet>().HasMany(x => x.Pages);

            modelBuilder.Entity<ExamBookletPage>().ToTable("ExamBookletPage");
            modelBuilder.Entity<ExamBookletPage>().HasKey(x => x.Id);
            modelBuilder.Entity<ExamBookletPage>().HasIndex(x => x.QrCode).IsUnique();

            modelBuilder.Entity<Task>().ToTable("Task");
            modelBuilder.Entity<Task>().HasKey(x => x.Id);

            modelBuilder.Entity<Submission>().ToTable("Submission");
            modelBuilder.Entity<Submission>().HasKey(x => x.Id);
            modelBuilder.Entity<Submission>().HasOne<Student>().WithMany();
            modelBuilder.Entity<Submission>().HasOne<ExamBooklet>().WithMany();
            modelBuilder.Entity<Submission>().HasMany(x => x.Answers);
            modelBuilder.Entity<Submission>().HasMany(x => x.Pages);
            modelBuilder.Entity<Submission>().HasMany(x => x.Remarks);

            modelBuilder.Entity<SubmissionPage>().ToTable("SubmissionPage");
            modelBuilder.Entity<SubmissionPage>().HasKey(x => x.Id);

            modelBuilder.Entity<Answer>().ToTable("Answer");
            modelBuilder.Entity<Answer>().HasKey(x => x.Id);
            modelBuilder.Entity<Answer>().HasOne<ExamTask>().WithMany();
            modelBuilder.Entity<Answer>().HasMany(x => x.Remarks);

            modelBuilder.Entity<Remark>().ToTable("Remark");
            modelBuilder.Entity<Remark>().HasKey(x => x.Id);
            modelBuilder.Entity<Remark>().HasOne(x => x.Input).WithOne().HasForeignKey<Remark>("InputId");
            modelBuilder.Entity<Remark>().HasOne<Teacher>().WithMany();

            modelBuilder.Entity<Input>().ToTable("Input");
            modelBuilder.Entity<Input>().HasKey(x => x.Id);
            modelBuilder.Entity<Input>().HasDiscriminator();
            
            modelBuilder.Entity<TextInput>();
        }

        private void OwnsAddress<TEntity>(ModelBuilder modelBuilder,
            Expression<Func<TEntity, Address?>> navigationExpression) where TEntity : class
        {
            modelBuilder.Entity<TEntity>().OwnsOne(navigationExpression, x =>
            {
                x.Property(y => y.StreetName).HasColumnName("StreetName");
                x.Property(y => y.StreetNumber).HasColumnName("StreetNumber");
                x.Property(y => y.PostCode).HasColumnName("PostalCode");
                x.Property(y => y.City).HasColumnName("City");
                x.Property(y => y.Country).HasColumnName("Country");
            });
        }
    }
}