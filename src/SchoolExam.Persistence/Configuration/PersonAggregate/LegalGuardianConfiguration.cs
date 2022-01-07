using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.PersonAggregate;

namespace SchoolExam.Persistence.Configuration.PersonAggregate;

public class LegalGuardianConfiguration : IEntityTypeConfiguration<LegalGuardian>
{
    public void Configure(EntityTypeBuilder<LegalGuardian> builder)
    {
    }
}