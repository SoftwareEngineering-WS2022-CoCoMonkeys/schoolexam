using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.Persistence.Configuration.ExamAggregate;

public class ExamParticipantConfiguration : IEntityTypeConfiguration<ExamParticipant>
{
    public void Configure(EntityTypeBuilder<ExamParticipant> builder)
    {
        builder.HasKey(x => new {x.ExamId, x.ParticipantId});
    }
}