using System.Linq;
using System.Threading.Tasks;
using SchoolExam.SharedKernel;

namespace Common.Infrastructure.EFAbstractions
{
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

        public void Add<TEntity, TIdentity>(TEntity entity) where TEntity : EntityBase<TIdentity>
        {
            Context.Add(entity);
        }

        public void Update<TEntity, TIdentity>(TEntity entity) where TEntity : EntityBase<TIdentity>
        {
            Context.Update(entity);
        }

        public void Remove<TEntity, TIdentity>(TEntity entity) where TEntity : EntityBase<TIdentity>
        {
            Context.Remove(entity);
        }
    }
}