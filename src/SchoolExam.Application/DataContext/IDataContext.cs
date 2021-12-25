using SchoolExam.Domain.Base;

namespace SchoolExam.Application.DataContext;

public interface IDataContext : IDisposable
{
    Task<int> SaveChangesAsync();
    TEntity? Find<TEntity, TIdentity>(TIdentity id) where TEntity : class, IEntity<TIdentity>;
    TEntity? Find<TEntity, TIdentity>(TIdentity id, params string[] includes)
        where TEntity : class, IEntity<TIdentity>;
    void Add<TEntity>(TEntity entity) where TEntity : class;
    void Update<TEntity>(TEntity entity) where TEntity : class;
    void Remove<TEntity>(TEntity entity) where TEntity : class;
}