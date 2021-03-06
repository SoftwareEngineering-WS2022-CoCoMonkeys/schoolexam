using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.Persistence.Configuration.ExamAggregate;

public class BookletPdfFileConfiguration : IEntityTypeConfiguration<BookletPdfFile>
{
    public void Configure(EntityTypeBuilder<BookletPdfFile> builder)
    {
    }
}