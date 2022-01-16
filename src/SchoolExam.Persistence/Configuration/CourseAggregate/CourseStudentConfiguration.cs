using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.CourseAggregate;

namespace SchoolExam.Persistence.Configuration.CourseAggregate;

public class CourseStudentConfiguration : IEntityTypeConfiguration<CourseStudent>
{
    public void Configure(EntityTypeBuilder<CourseStudent> builder)
    {
        builder.ToTable("CourseStudent");
        builder.HasKey(x => new {x.CourseId, x.StudentId});
        builder.HasOne(x => x.Course).WithMany(x => x.Students).HasForeignKey(x => x.CourseId);
        builder.HasOne(x => x.Student).WithMany(x => x.Courses).HasForeignKey(x => x.StudentId);
    }
}