using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Persistence.Configuration.SubmissionAggregate;

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.ToTable("Submission");
        builder.HasKey(x => x.Id);
        builder.HasOne<Student>().WithMany();
        builder.HasOne<ExamBooklet>().WithMany();
        builder.HasMany(x => x.Answers);
        builder.HasMany(x => x.Pages).WithOne().HasForeignKey(x => x.SubmissionId);
        builder.HasOne(x => x.PdfFile).WithOne().HasForeignKey<SubmissionPdfFile>(x => x.SubmissionId);
    }
}