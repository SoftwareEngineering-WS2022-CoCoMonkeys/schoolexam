using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Extensions;
using SchoolExam.Persistence.Extensions;

namespace SchoolExam.Persistence.Configuration.PersonAggregate;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("Person");
        builder.HasDiscriminator();
        builder.HasKey(x => x.Id);
        builder.OwnsAddress(x => x.Address, true, new
        {
            PersonId = SeedIds.BriggiteSchweinebauerId,
            StreetName = "Klarer-Kopf-Weg",
            StreetNumber = "1a",
            PostCode = "20095",
            City = "Hamburg",
            Country = "Deutschland"
        }, new
        {
            PersonId = SeedIds.AmiraJabbarId,
            StreetName = "You-Go-Girl-Allee",
            StreetNumber = "99",
            PostCode = "80333",
            City = "München",
            Country = "Deutschland"
        });
        builder.HasData(new
        {
            Id = SeedIds.BriggiteSchweinebauerId, FirstName = "Briggite", LastName = "Schweinebauer",
            DateOfBirth = new DateTime(1974, 5, 18).SetKindUtc(), EmailAddress = "thorsten.thurn@school-exam.de",
            SchoolId = SeedIds.GymnasiumDiedorfId, Discriminator = "Teacher"
        }, new
        {
            Id = SeedIds.AmiraJabbarId, FirstName = "Amira", LastName = "Jabbar",
            DateOfBirth = new DateTime(2004, 7, 29).SetKindUtc(), EmailAddress = "amira.jabbar@school-exam.de",
            SchoolId = SeedIds.GymnasiumDiedorfId, Discriminator = "Student"
        });
    }
}