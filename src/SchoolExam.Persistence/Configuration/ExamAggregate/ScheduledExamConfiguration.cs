using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Persistence.Extensions;

namespace SchoolExam.Persistence.Configuration.ExamAggregate;

public class ScheduledExamConfiguration: IEntityTypeConfiguration<ScheduledExam>
{
    public void Configure(EntityTypeBuilder<ScheduledExam> builder)
    {
        builder.ToTable("ScheduledExam");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Exam).WithOne().HasForeignKey<ScheduledExam>(x  => x.ExamId);
    }
}