using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Persistence.Extensions;

public static class SubjectExtensions
{
    public static EntityTypeBuilder<TEntity> OwnsSubject<TEntity>(this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, Subject?>> navigationExpression, bool isRequired = true) where TEntity : class
    {
        return builder.OwnsOne(navigationExpression,
            x => { x.Property(s => s.Name).HasColumnName("Subject").IsRequired(isRequired); });
    }
}