using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Persistence.Extensions;

namespace SchoolExam.Persistence.Configuration.ExamAggregate;

public class BookletPageConfiguration : IEntityTypeConfiguration<BookletPage>
{
    public void Configure(EntityTypeBuilder<BookletPage> builder)
    {
        builder.ToTable("BookletPage");
        builder.HasKey(x => x.Id);
        builder.OwnsQrCode(x => x.QrCode);
    }
}