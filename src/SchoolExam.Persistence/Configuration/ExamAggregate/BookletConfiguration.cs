using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.Persistence.Configuration.ExamAggregate;

public class BookletConfiguration : IEntityTypeConfiguration<Booklet>
{
    public void Configure(EntityTypeBuilder<Booklet> builder)
    {
        builder.ToTable("Booklet");
        builder.HasKey(x => x.Id);
        builder.HasMany(x => x.Pages).WithOne().HasForeignKey(x => x.BookletId);
        builder.HasOne(x => x.PdfFile).WithOne().HasForeignKey<BookletPdfFile>(x => x.BookletId);
    }
}