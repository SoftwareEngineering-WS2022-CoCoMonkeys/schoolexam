using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Persistence.Extensions;

public static class GradingTableIntervalExtensions
{
    public static EntityTypeBuilder<GradingTable> OwnsGradingTableIntervals(
        this EntityTypeBuilder<GradingTable> builder,
        Expression<Func<GradingTable, IEnumerable<GradingTableInterval>?>> navigationExpression)
    {
        return builder.OwnsMany(navigationExpression,
            x =>
            {
                x.WithOwner(y => y.GradingTable).HasForeignKey(y => y.GradingTableId);
                x.OwnsOne(y => y.Start, y =>
                {
                    y.Property(z => z.Points).HasColumnName("StartPoints");
                    y.Property(z => z.Type).HasColumnName("StartType");
                });
                x.OwnsOne(y => y.End, y =>
                {
                    y.Property(z => z.Points).HasColumnName("EndPoints");
                    y.Property(z => z.Type).HasColumnName("EndType");
                });
                x.Property(y => y.Grade).HasColumnName("Grade");
                x.Property(y => y.Type).HasColumnName("Type");
            });
    }
}