using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Base;

namespace SchoolExam.Persistence.Configuration;

public class FileBaseConfiguration : IEntityTypeConfiguration<FileBase>
{
    public void Configure(EntityTypeBuilder<FileBase> builder)
    {
        builder.ToTable("File");
        builder.HasKey(x => x.Id);
        builder.HasDiscriminator();
    }
}