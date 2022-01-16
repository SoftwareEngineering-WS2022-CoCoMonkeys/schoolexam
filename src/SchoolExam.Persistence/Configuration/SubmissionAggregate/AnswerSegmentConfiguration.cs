using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.SubmissionAggregate;
using SchoolExam.Persistence.Extensions;

namespace SchoolExam.Persistence.Configuration.SubmissionAggregate;

public class AnswerSegmentConfiguration : IEntityTypeConfiguration<AnswerSegment>
{
    public void Configure(EntityTypeBuilder<AnswerSegment> builder)
    {
        builder.ToTable("AnswerSegment");
        builder.HasKey(x => x.Id);
        builder.OwnsExamPosition(x => x.Start, x => x.End);
    }
}