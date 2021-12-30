using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.SchoolAggregate;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Persistence.Extensions;
using SchoolExam.Persistence.Seed;

namespace SchoolExam.Persistence.Configuration;

public class SchoolConfiguration : IEntityTypeConfiguration<School>
{
    public void Configure(EntityTypeBuilder<School> builder)
    {
        builder.ToTable("School");
        builder.HasKey(x => x.Id);
        builder.OwnsAddress(x => x.Location);

        // builder.HasData(new School(SeedIds.GymnasiumDiedorfId, "Schmuttertal-Gymnasium Diedorf",
        //     new Address("Schmetterlingsplatz", "1", "86420", "Diedorf", "Deutschland")));
    }
}