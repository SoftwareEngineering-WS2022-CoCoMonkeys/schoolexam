using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.PersonAggregate;

namespace SchoolExam.Persistence.Configuration;

public class StudentLegalGuardianConfiguration : IEntityTypeConfiguration<StudentLegalGuardian>
{
    public void Configure(EntityTypeBuilder<StudentLegalGuardian> builder)
    {
        builder.ToTable("StudentLegalGuardian");
        builder.HasKey(x => new {x.StudentId, x.LegalGuardianId});
        builder.HasOne<Student>().WithMany(Student.LegalGuardiansName).HasForeignKey(x => x.StudentId);
        builder.HasOne<LegalGuardian>().WithMany(LegalGuardian.ChildrenName).HasForeignKey(x => x.LegalGuardianId);
    }
}