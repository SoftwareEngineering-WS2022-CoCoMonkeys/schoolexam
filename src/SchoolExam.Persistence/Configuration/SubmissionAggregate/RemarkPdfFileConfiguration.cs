using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Persistence.Configuration.SubmissionAggregate;

public class RemarkPdfFileConfiguration : IEntityTypeConfiguration<RemarkPdfFile>
{
    public void Configure(EntityTypeBuilder<RemarkPdfFile> builder)
    {
    }
}