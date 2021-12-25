using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Persistence.Extensions;

namespace SchoolExam.Persistence.Configuration;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("Person");
        builder.HasDiscriminator();
        builder.HasKey(x => x.Id);
        builder.OwnsAddress(x => x.Address);
    }
}