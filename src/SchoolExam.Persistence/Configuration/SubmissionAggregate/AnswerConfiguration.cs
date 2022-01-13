using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Persistence.Configuration.SubmissionAggregate;

public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.ToTable("Answer");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Task).WithMany().HasForeignKey(x => x.TaskId);
        builder.HasMany(x => x.Segments).WithOne().HasForeignKey(x => x.AnswerId);
    }
}