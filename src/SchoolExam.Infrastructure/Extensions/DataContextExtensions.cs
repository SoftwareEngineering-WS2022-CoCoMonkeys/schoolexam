using SchoolExam.Application.DataContext;
using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Base;

namespace SchoolExam.Infrastructure.Extensions;

public static class DataContextExtensions
{
    public static TEntity? Find<TEntity, TIdentity>(this IDataContext dataContext, TIdentity id)
        where TEntity : class, IEntity<TIdentity>
    {
        var spec = new EntityByIdSpecification<TEntity, TIdentity>(id);
        return dataContext.Find(spec);
    }
    
    public static IEnumerable<TEntity> List<TEntity>(this IDataContext dataContext) where TEntity : class
    {
        var spec = new EntitySpecification<TEntity>();
        return dataContext.List(spec);
    }
}