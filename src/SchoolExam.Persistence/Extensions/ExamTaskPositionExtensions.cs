using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Persistence.Extensions;

public static class ExamTaskPositionExtensions
{
    public static EntityTypeBuilder<TEntity> OwnsExamTaskPosition<TEntity>(this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, ExamTaskPosition?>> navigationExpression) where TEntity : class
    {
        return builder.OwnsOne(navigationExpression, x =>
        {
            x.OwnsOne(y => y.Start, y =>
            {
                y.Property(z => z.Page).HasColumnName("StartPage");
                y.Property(z => z.Y).HasColumnName("StartY");
            });
            x.OwnsOne(y => y.End, y =>
            {
                y.Property(z => z.Page).HasColumnName("EndPage");
                y.Property(z => z.Y).HasColumnName("EndY");
            });
        });
    }
}