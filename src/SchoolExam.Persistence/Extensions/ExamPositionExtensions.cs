using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Persistence.Extensions;

public static class ExamPositionExtensions
{
    public static EntityTypeBuilder<TEntity> OwnsExamPosition<TEntity>(this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, ExamPosition?>> navigationExpressionStart,
        Expression<Func<TEntity, ExamPosition?>> navigationExpressionEnd) where TEntity : class
    {
        return builder.OwnsOne(navigationExpressionStart, x =>
        {
            x.Property(y => y.Page).HasColumnName("StartPage");
            x.Property(y => y.Y).HasColumnName("StartY");
        }).OwnsOne(navigationExpressionEnd, x =>
        {
            x.Property(y => y.Page).HasColumnName("EndPage");
            x.Property(y => y.Y).HasColumnName("EndY");
        });
    }
}