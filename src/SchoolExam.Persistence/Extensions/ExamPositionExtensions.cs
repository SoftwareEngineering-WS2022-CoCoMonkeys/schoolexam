using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Persistence.Extensions;

public static class ExamPositionExtensions
{
    public static EntityTypeBuilder<TEntity> OwnsExamPosition<TEntity>(this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, ExamPosition?>> navigationExpression) where TEntity : class
    {
        return builder.OwnsOne(navigationExpression, x =>
        {
            x.Property(y => y.Page).HasColumnName("Page");
            x.Property(y => y.Y).HasColumnName("Y");
        });
    }
}