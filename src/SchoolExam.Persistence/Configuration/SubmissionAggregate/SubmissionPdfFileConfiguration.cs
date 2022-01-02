using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Persistence.Configuration.SubmissionAggregate;

public class SubmissionPdfFileConfiguration : IEntityTypeConfiguration<SubmissionPdfFile>
{
    public void Configure(EntityTypeBuilder<SubmissionPdfFile> builder)
    {
    }
}