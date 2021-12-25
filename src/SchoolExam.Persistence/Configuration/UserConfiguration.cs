using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.Entities.UserAggregate;
using SchoolExam.Persistence.Extensions;

namespace SchoolExam.Persistence.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");
        builder.HasKey(x => x.Id);
        builder.HasOne<Person>().WithOne().HasForeignKey<User>(x => x.PersonId).IsRequired(false);
        builder.OwnsRole(x => x.Role);
    }
}