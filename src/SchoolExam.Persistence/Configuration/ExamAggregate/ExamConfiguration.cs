using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Extensions;

namespace SchoolExam.Persistence.Configuration.ExamAggregate;

public class ExamConfiguration : IEntityTypeConfiguration<Exam>
{
    public void Configure(EntityTypeBuilder<Exam> builder)
    {
        builder.ToTable("Exam");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.GradingTable);
        builder.HasMany(x => x.Tasks);
        builder.HasMany(x => x.Booklets).WithOne(x => x.Exam).HasForeignKey(x => x.ExamId);
        builder.HasOne(x => x.TaskPdfFile).WithOne().HasForeignKey<TaskPdfFile>(x => x.ExamId);
        builder.HasData(new Exam(SeedIds.ProjektmanagementExamId, "Projektmanagement",
            "Mündliche Leistungsfeststellung", new DateTime(2022, 4, 1).SetKindUtc(), SeedIds.BriggiteSchweinebauerId,
            SeedIds.SozialwissenschaftenCourseId));
    }
}