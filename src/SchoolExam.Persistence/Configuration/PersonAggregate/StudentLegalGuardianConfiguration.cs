using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.PersonAggregate;

namespace SchoolExam.Persistence.Configuration.PersonAggregate;

public class StudentLegalGuardianConfiguration : IEntityTypeConfiguration<StudentLegalGuardian>
{
    public void Configure(EntityTypeBuilder<StudentLegalGuardian> builder)
    {
        builder.ToTable("StudentLegalGuardian");
        builder.HasKey(x => new {x.StudentId, x.LegalGuardianId});
        builder.HasOne<Student>().WithMany(x => x.LegalGuardians).HasForeignKey(x => x.StudentId);
        builder.HasOne<LegalGuardian>().WithMany(x => x.Children).HasForeignKey(x => x.LegalGuardianId);
    }
}