using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.SchoolAggregate;
using SchoolExam.Persistence.Extensions;

namespace SchoolExam.Persistence.Configuration;

public class SchoolConfiguration : IEntityTypeConfiguration<School>
{
    public void Configure(EntityTypeBuilder<School> builder)
    {
        builder.ToTable("School");
        builder.HasKey(x => x.Id);
        builder.OwnsAddress(x => x.Location);
    }
}