using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.Entities.PersonAggregate;

namespace SchoolExam.Persistence.Configuration.CourseAggregate;

public class CourseStudentConfiguration : IEntityTypeConfiguration<CourseStudent>
{
    public void Configure(EntityTypeBuilder<CourseStudent> builder)
    {
        builder.ToTable("CourseStudent");
        builder.HasKey(x => new {x.CourseId, x.StudentId});
        builder.HasOne<Course>().WithMany(Course.StudentsName).HasForeignKey(x => x.CourseId);
        builder.HasOne<Student>().WithMany(Student.CoursesName).HasForeignKey(x => x.StudentId);
    }
}