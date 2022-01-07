using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Base;
using SchoolExam.Domain.Entities.UserAggregate;

namespace SchoolExam.Persistence.Configuration;

public class FileBaseConfiguration : IEntityTypeConfiguration<FileBase>
{
    public void Configure(EntityTypeBuilder<FileBase> builder)
    {
        builder.ToTable("File");
        builder.HasKey(x => x.Id);
        builder.HasDiscriminator();
        builder.HasOne<User>(x => x.Uploader).WithMany().HasForeignKey(x => x.UploaderId);
    }
}