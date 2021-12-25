using Microsoft.EntityFrameworkCore;
using SchoolExam.Application.DataContext;
using SchoolExam.Domain.Base;

namespace SchoolExam.Persistence.Base;

public abstract class DataContextBase<TContext> : IDataContext where TContext : DbContextBase
{
    protected TContext Context { get; }

    public DataContextBase(TContext context)
    {
        Context = context;
    }
        
    public async Task<int> SaveChangesAsync()
    {
        return await Context.SaveChangesAsync();
    }

    public TEntity? Find<TEntity, TIdentity>(TIdentity id) where TEntity : class, IEntity<TIdentity>
    {
        return Context.Find<TEntity>(id);
    }

    public TEntity? Find<TEntity, TIdentity>(TIdentity id, params string[] includes) where TEntity : class, IEntity<TIdentity>
    {
        var set = Context.Set<TEntity>();
        IQueryable<TEntity> query = set;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

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
}