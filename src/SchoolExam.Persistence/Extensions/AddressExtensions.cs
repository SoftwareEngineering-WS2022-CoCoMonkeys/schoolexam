using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Persistence.Extensions;

public static class AddressExtensions
{
    public static EntityTypeBuilder<TEntity> OwnsAddress<TEntity>(this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, Address?>> navigationExpression, bool isRequired, params object[] data) where TEntity : class
    {
        return builder.OwnsOne(navigationExpression,
            x =>
            {
                x.Property(y => y.StreetName).HasColumnName("StreetName").IsRequired(isRequired);
                x.Property(y => y.StreetNumber).HasColumnName("StreetNumber").IsRequired(isRequired);
                x.Property(y => y.PostCode).HasColumnName("PostalCode").IsRequired(isRequired);
                x.Property(y => y.City).HasColumnName("City").IsRequired(isRequired);
                x.Property(y => y.Country).HasColumnName("Country").IsRequired(isRequired);
                x.HasData(data);
            });
    }
}