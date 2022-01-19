using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Extensions;
using SchoolExam.Persistence.Extensions;

namespace SchoolExam.Persistence.Configuration.ExamAggregate;

public class ExamConfiguration : IEntityTypeConfiguration<Exam>
{
    public void Configure(EntityTypeBuilder<Exam> builder)
    {
        builder.ToTable("Exam");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.GradingTable).WithOne().HasForeignKey<GradingTable>(x => x.ExamId);
        builder.HasMany(x => x.Tasks).WithOne().HasForeignKey(x => x.ExamId);
        builder.HasMany(x => x.Booklets).WithOne(x => x.Exam).HasForeignKey(x => x.ExamId);
        builder.HasOne(x => x.TaskPdfFile).WithOne().HasForeignKey<TaskPdfFile>(x => x.ExamId);
        builder.OwnsTopic(x => x.Topic, true);
    }
}