using SchoolExam.Application.Repository;
using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Base;

namespace SchoolExam.Infrastructure.Extensions;

public static class RepositoryExtensions
{
    public static TEntity? Find<TEntity>(this IRepository repository, Guid id) where TEntity : class, IEntity
    {
        var spec = new EntityByIdSpecification<TEntity>(id);
        return repository.Find(spec);
    }

    public static IEnumerable<TEntity> List<TEntity>(this IRepository repository) where TEntity : class
    {
        var spec = new EntitySpecification<TEntity>();
        return repository.List(spec);
    }

    public static IEnumerable<TEntity> List<TEntity, TSpec>(this IRepository repository)
        where TEntity : class where TSpec : ISpecification<TEntity>, new()
    {
        var spec = new TSpec();
        return repository.List(spec);
    }
    
    public static IEnumerable<TEntity> List<TEntity>(this IRepository repository, HashSet<Guid> ids)
        where TEntity : class, IEntity
    {
        var spec = new EntityByIdsSpecification<TEntity>(ids);
        return repository.List(spec);
    }
}