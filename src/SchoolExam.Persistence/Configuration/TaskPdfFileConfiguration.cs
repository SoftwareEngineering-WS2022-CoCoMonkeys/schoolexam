using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.Persistence.Configuration;

public class TaskPdfFileConfiguration : IEntityTypeConfiguration<TaskPdfFile>
{
    public void Configure(EntityTypeBuilder<TaskPdfFile> builder)
    {
    }
}