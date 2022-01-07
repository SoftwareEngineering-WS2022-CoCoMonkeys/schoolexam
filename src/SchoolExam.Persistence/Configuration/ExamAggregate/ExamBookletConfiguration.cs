using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.Persistence.Configuration.ExamAggregate;

public class ExamBookletConfiguration : IEntityTypeConfiguration<ExamBooklet>
{
    public void Configure(EntityTypeBuilder<ExamBooklet> builder)
    {
        builder.ToTable("ExamBooklet");
        builder.HasKey(x => x.Id);
        builder.HasMany(x => x.Pages).WithOne().HasForeignKey(x => x.BookletId);
        builder.HasOne(x => x.PdfFile).WithOne().HasForeignKey<BookletPdfFile>(x => x.BookletId);
    }
}