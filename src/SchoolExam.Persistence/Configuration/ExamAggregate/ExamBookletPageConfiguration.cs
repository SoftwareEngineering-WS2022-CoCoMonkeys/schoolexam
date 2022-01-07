using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.Persistence.Configuration.ExamAggregate;

public class ExamBookletPageConfiguration : IEntityTypeConfiguration<ExamBookletPage>
{
    public void Configure(EntityTypeBuilder<ExamBookletPage> builder)
    {
        builder.ToTable("ExamBookletPage");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.QrCodeData).IsUnique();
        builder.Property(x => x.QrCodeData).HasMaxLength(32).IsFixedLength();
    }
}