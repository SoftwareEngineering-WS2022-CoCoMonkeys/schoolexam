using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.Entities.UserAggregate;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Persistence.Extensions;

namespace SchoolExam.Persistence.Configuration.UserAggregate;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");
        builder.HasKey(x => x.Id);
        builder.HasOne<Person>().WithOne().HasForeignKey<User>(x => x.PersonId).IsRequired(false);
        builder.OwnsRole(x => x.Role, new {UserId = SeedIds.BriggiteSchweinebauerUserId, Name = Role.TeacherName});
        builder.HasData(new
        {
            Id = SeedIds.BriggiteSchweinebauerUserId,
            Username = "admin",
            Password = "$2a$11$3Q8Re.PhjBIPqPIqzAy3Y./XFRjcelEOr7kL0X27ljVbay1PwTMw2",
            PersonId = SeedIds.BriggiteSchweinebauerId,
        });
    }
}