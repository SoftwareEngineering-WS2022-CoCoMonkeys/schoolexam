using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Persistence.Extensions;

namespace SchoolExam.Persistence.Configuration.ExamAggregate;

public class GradingTableConfiguration : IEntityTypeConfiguration<GradingTable>
{
    public void Configure(EntityTypeBuilder<GradingTable> builder)
    {
        builder.ToTable("GradingTable");
        builder.HasKey(x => x.Id);
        builder.OwnsGradingTableIntervals(x => x.Intervals);
    }
}