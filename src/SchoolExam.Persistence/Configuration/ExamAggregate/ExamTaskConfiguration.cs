using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Persistence.Extensions;

namespace SchoolExam.Persistence.Configuration.ExamAggregate;

public class ExamTaskConfiguration : IEntityTypeConfiguration<ExamTask>
{
    public void Configure(EntityTypeBuilder<ExamTask> builder)
    {
        builder.ToTable("ExamTask");
        builder.HasKey(x => x.Id);
        builder.OwnsExamPosition(x => x.Position);
    }
}