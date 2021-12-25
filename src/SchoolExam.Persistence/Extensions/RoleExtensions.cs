using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Persistence.Extensions;

public static class RoleExtensions
{
    public static EntityTypeBuilder<TEntity> OwnsRole<TEntity>(this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, Role?>> navigationExpression) where TEntity : class
    {
        return builder.OwnsOne(navigationExpression, x => { x.Property(s => s.Name).HasColumnName("Role"); });
    }
}