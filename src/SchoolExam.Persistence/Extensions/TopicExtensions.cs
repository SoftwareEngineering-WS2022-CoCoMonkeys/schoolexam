using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Persistence.Extensions;

public static class TopicExtensions
{
    public static EntityTypeBuilder<TEntity> OwnsTopic<TEntity>(this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, Topic?>> navigationExpression, bool isRequired, params object[] data)
        where TEntity : class
    {
        return builder.OwnsOne(navigationExpression, x =>
        {
            x.Property(s => s.Name).HasColumnName("Topic").IsRequired(isRequired);
            x.HasData(data);
        });
    }
}