using System.Threading.Tasks;
using SchoolExam.SharedKernel;

namespace Common.Infrastructure.EFAbstractions
{
    public interface IDataContext
    {
        Task<int> SaveChangesAsync();
        void Add<TEntity, TIdentity>(TEntity entity) where TEntity : EntityBase<TIdentity>;
        void Update<TEntity, TIdentity>(TEntity entity) where TEntity : EntityBase<TIdentity>;
        void Remove<TEntity, TIdentity>(TEntity entity) where TEntity : EntityBase<TIdentity>;
    }
}