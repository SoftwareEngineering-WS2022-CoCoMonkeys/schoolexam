using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Persistence.Extensions;

public static class GradingTableIntervalExtensions
{
    public static EntityTypeBuilder<TEntity> OwnsGradingTableIntervals<TEntity>(this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, IEnumerable<GradingTableInterval>?>> navigationExpression) where TEntity : class
    {
        return builder.OwnsMany(navigationExpression,
            x =>
            {
                x.WithOwner().HasForeignKey("GradingTableId");
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
            });
    }
}