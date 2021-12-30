using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Persistence.Configuration;

public class SubmissionPagePdfFileConfiguration : IEntityTypeConfiguration<SubmissionPagePdfFile>
{
    public void Configure(EntityTypeBuilder<SubmissionPagePdfFile> builder)
    {
    }
}