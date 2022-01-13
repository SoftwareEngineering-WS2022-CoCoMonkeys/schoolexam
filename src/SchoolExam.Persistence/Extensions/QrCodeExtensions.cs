using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Persistence.Extensions;

public static class QrCodeExtensions
{
    public static EntityTypeBuilder<TEntity> OwnsQrCode<TEntity>(this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, QrCode?>> navigationExpression, params object[] data) where TEntity : class
    {
        return builder.OwnsOne(navigationExpression, x =>
        {
            x.Property(q => q.Data).HasColumnName("QrCodeData");
            x.HasIndex(q => q.Data).IsUnique();
            x.HasData(data);
        });
    }
}