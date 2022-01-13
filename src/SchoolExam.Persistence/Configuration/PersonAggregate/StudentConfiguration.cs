using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Persistence.Extensions;

namespace SchoolExam.Persistence.Configuration.PersonAggregate;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.OwnsQrCode(x => x.QrCode,
            new {StudentId = SeedIds.AmiraJabbarId, Data = "d18b19227701139f25eb4f205f785995"});
    }
}