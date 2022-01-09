using SchoolExam.Application.Specifications;

namespace SchoolExam.Application.DataContext;

public interface IDataContext : IDisposable
{
    Task<int> SaveChangesAsync();
    IEnumerable<TEntity> List<TEntity>(ISpecification<TEntity> spec) where TEntity : class;
    TEntity? Find<TEntity>(ISpecification<TEntity> spec) where TEntity : class;
    void Add<TEntity>(TEntity entity) where TEntity : class;
    void Update<TEntity>(TEntity entity) where TEntity : class;
    void Remove<TEntity>(TEntity entity) where TEntity : class;
}