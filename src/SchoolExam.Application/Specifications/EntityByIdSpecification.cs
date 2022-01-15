using SchoolExam.Domain.Base;

namespace SchoolExam.Application.Specifications;

public class EntityByIdSpecification<TEntity> : EntitySpecification<TEntity>
    where TEntity : class, IEntity
{
    public EntityByIdSpecification(Guid id) : base(x => x.Id.Equals(id))
    {
    }
}