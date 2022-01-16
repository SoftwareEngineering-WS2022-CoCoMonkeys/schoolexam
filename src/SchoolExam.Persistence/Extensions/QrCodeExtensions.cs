using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Persistence.Extensions;

public static class QrCodeExtensions
{
    public static EntityTypeBuilder<TEntity> OwnsQrCode<TEntity>(this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, QrCode?>> navigationExpression, bool isUnique = true, params object[] data) where TEntity : class
    {
        return builder.OwnsOne(navigationExpression, x =>
        {
            x.Property(q => q.Data).HasColumnName("QrCodeData");
            var indexBuilder = x.HasIndex(q => q.Data);
            if (isUnique)
            {
                indexBuilder.IsUnique();
            }
            x.HasData(data);
        });
    }
}