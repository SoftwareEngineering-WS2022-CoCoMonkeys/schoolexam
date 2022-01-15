using Microsoft.EntityFrameworkCore;
using SchoolExam.Application.Repository;
using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Base;

namespace SchoolExam.Persistence.Base;

public abstract class RepositoryBase<TContext> : IRepository where TContext : DbContextBase
{
    protected TContext Context { get; }

    public RepositoryBase(TContext context)
    {
        Context = context;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await Context.SaveChangesAsync();
    }

    public IEnumerable<TEntity> List<TEntity>(ISpecification<TEntity> spec) where TEntity : class
    {
        var queryable = GetQueryableFromSpec(spec).AsSplitQuery();

        return queryable.Where(spec.Criteria).AsEnumerable();
    }

    public TEntity? Find<TEntity>(ISpecification<TEntity> spec) where TEntity : class
    {
        var queryable = GetQueryableFromSpec(spec).AsSplitQuery();
        return queryable.SingleOrDefault(spec.Criteria);
    }

    public TEntity? Find<TEntity, TIdentity>(TIdentity id, params string[] includes)
        where TEntity : class, IEntity<TIdentity>
    {
        var queryable = Context.Set<TEntity>().AsQueryable();
        IQueryable<TEntity> query = includes.Aggregate(queryable, (current, include) => current.Include(include));
        return query.SingleOrDefault(x => x.Id.Equals(id));
    }

    public void Add<TEntity>(TEntity entity) where TEntity : class
    {
        Context.Add(entity);
    }

    public void Update<TEntity>(TEntity entity) where TEntity : class
    {
        Context.Update(entity);
    }

    public void Remove<TEntity>(TEntity entity) where TEntity : class
    {
        Context.Remove(entity);
    }

    public void Dispose()
    {
        Context.Dispose();
    }

    private IQueryable<TEntity> GetQueryableFromSpec<TEntity>(ISpecification<TEntity> spec) where TEntity : class
    {
        var queryable = Context.Set<TEntity>().AsQueryable();
        var queryableResultWithIncludes =
            spec.Includes.Aggregate(queryable, (current, include) => current.Include(include));
        var queryableResultWithIncludeStrings = spec.IncludeStrings.Aggregate(queryableResultWithIncludes,
            (current, include) => current.Include(include));

        return queryableResultWithIncludeStrings;
    }
}