using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Persistence.Configuration;

public class SubmissionPageConfiguration : IEntityTypeConfiguration<SubmissionPage>
{
    public void Configure(EntityTypeBuilder<SubmissionPage> builder)
    {
        builder.ToTable("SubmissionPage");
        builder.HasKey(x => x.Id);
    }
}