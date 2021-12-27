using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.Persistence.Configuration;

public class ExamConfiguration : IEntityTypeConfiguration<Exam>
{
    public void Configure(EntityTypeBuilder<Exam> builder)
    {
        builder.ToTable("Exam");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.GradingTable);
        builder.HasMany(x => x.Tasks);
        builder.HasMany(x => x.Booklets);
        builder.HasOne(x => x.TaskPdfFile).WithOne().HasForeignKey<TaskPdfFile>(x => x.ExamId);
    }
}