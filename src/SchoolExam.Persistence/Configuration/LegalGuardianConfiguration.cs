using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.PersonAggregate;

namespace SchoolExam.Persistence.Configuration;

public class LegalGuardianConfiguration : IEntityTypeConfiguration<LegalGuardian>
{
    public void Configure(EntityTypeBuilder<LegalGuardian> builder)
    {
    }
}