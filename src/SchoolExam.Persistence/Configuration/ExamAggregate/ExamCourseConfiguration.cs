using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.Persistence.Configuration.ExamAggregate;

public class ExamCourseConfiguration : IEntityTypeConfiguration<ExamCourse>
{
    public void Configure(EntityTypeBuilder<ExamCourse> builder)
    {
        builder.HasOne(x => x.Course).WithMany().HasForeignKey(x => x.ParticipantId);
    }
}