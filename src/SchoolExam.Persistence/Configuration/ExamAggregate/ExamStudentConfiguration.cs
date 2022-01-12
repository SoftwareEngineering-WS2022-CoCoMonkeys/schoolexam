using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.Persistence.Configuration.ExamAggregate;

public class ExamStudentConfiguration : IEntityTypeConfiguration<ExamStudent>
{
    public void Configure(EntityTypeBuilder<ExamStudent> builder)
    {
        builder.ToTable("ExamStudent");
        builder.HasOne(x => x.Student).WithMany().HasForeignKey(x => x.ParticipantId);
    }
}