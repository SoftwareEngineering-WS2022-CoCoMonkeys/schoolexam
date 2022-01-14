using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;
using SchoolExam.Persistence.Extensions;

namespace SchoolExam.Persistence.Configuration.SubmissionAggregate;

public class SubmissionPageConfiguration : IEntityTypeConfiguration<SubmissionPage>
{
    public void Configure(EntityTypeBuilder<SubmissionPage> builder)
    {
        builder.ToTable("SubmissionPage");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.PdfFile).WithOne().HasForeignKey<SubmissionPagePdfFile>(x => x.SubmissionPageId);
        builder.HasOne<Exam>().WithMany().HasForeignKey(x => x.ExamId);
        builder.HasOne<BookletPage>().WithOne(x => x.SubmissionPage)
            .HasForeignKey<SubmissionPage>(x => x.BookletPageId);
        builder.OwnsQrCode(x => x.StudentQrCode);
    }
}