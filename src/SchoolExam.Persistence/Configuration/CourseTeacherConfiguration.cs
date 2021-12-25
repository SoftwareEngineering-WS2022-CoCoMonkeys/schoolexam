using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.Entities.PersonAggregate;

namespace SchoolExam.Persistence.Configuration;

public class CourseTeacherConfiguration : IEntityTypeConfiguration<CourseTeacher>
{
    public void Configure(EntityTypeBuilder<CourseTeacher> builder)
    {
        builder.ToTable("CourseTeacher");
        builder.HasKey(x => new {x.CourseId, x.TeacherId});
        builder.HasOne<Course>().WithMany(Course.TeachersName).HasForeignKey(x => x.CourseId);
        builder.HasOne<Teacher>().WithMany(Teacher.CoursesName).HasForeignKey(x => x.TeacherId);
    }
}