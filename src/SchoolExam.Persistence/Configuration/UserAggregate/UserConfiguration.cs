using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.UserAggregate;
using SchoolExam.Persistence.Extensions;

namespace SchoolExam.Persistence.Configuration.UserAggregate;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Username).IsRequired();
        builder.HasIndex("Username").IsUnique();
        builder.HasOne(x => x.Person).WithOne(x => x.User).HasForeignKey<User>(x => x.PersonId).IsRequired(false);
        builder.OwnsRole(x => x.Role);
    }   
}