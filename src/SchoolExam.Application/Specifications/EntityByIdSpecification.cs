using SchoolExam.Domain.Base;

namespace SchoolExam.Application.Specifications;

public class EntityByIdSpecification<TEntity, TIdentity> : EntitySpecification<TEntity>
    where TEntity : class, IEntity<TIdentity>
{
    public EntityByIdSpecification(TIdentity id) : base(x => x.Id.Equals(id))
    {
    }
}