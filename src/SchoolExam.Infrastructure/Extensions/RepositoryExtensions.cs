using SchoolExam.Application.Repository;
using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Base;

namespace SchoolExam.Infrastructure.Extensions;

public static class RepositoryExtensions
{
    public static TEntity? Find<TEntity, TIdentity>(this IRepository repository, TIdentity id)
        where TEntity : class, IEntity<TIdentity>
    {
        var spec = new EntityByIdSpecification<TEntity, TIdentity>(id);
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
}