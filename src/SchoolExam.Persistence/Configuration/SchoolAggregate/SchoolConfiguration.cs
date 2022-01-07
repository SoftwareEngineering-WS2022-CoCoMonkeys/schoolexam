using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.SchoolAggregate;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Persistence.Extensions;

namespace SchoolExam.Persistence.Configuration.SchoolAggregate;

public class SchoolConfiguration : IEntityTypeConfiguration<School>
{
    public void Configure(EntityTypeBuilder<School> builder)
    {
        builder.ToTable("School");
        builder.HasKey(x => x.Id);

        builder.HasData(new School(SeedIds.GymnasiumDiedorfId, "Schmuttertal-Gymnasium Diedorf", null));

        builder.OwnsAddress(x => x.Location, true, new
        {
            SchoolId = SeedIds.GymnasiumDiedorfId,
            StreetName = "Schmetterlingsplatz",
            StreetNumber = "1",
            PostCode = "86420",
            City = "Diedorf",
            Country = "Deutschland"
        });
    }
}