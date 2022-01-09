using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.Entities.PersonAggregate;

namespace SchoolExam.Persistence.Configuration.CourseAggregate;

public class CourseTeacherConfiguration : IEntityTypeConfiguration<CourseTeacher>
{
    public void Configure(EntityTypeBuilder<CourseTeacher> builder)
    {
        builder.ToTable("CourseTeacher");
        builder.HasKey(x => new {x.CourseId, x.TeacherId});
        builder.HasOne(x => x.Course).WithMany(x => x.Teachers).HasForeignKey(x => x.CourseId);
        builder.HasOne<Teacher>().WithMany(x => x.Courses).HasForeignKey(x => x.TeacherId);
        builder.HasData(new CourseTeacher(SeedIds.SozialwissenschaftenCourseId, SeedIds.BriggiteSchweinebauerId));
    }
}