using SchoolExam.Domain.Base;

namespace SchoolExam.Application.Specifications;

public class EntityByIdsSpecification<TEntity, TIdentity> : EntitySpecification<TEntity>
    where TEntity : class, IEntity<TIdentity>
{
    public EntityByIdsSpecification(HashSet<TIdentity> ids) : base(x => ids.Contains(x.Id))
    {
    }
}