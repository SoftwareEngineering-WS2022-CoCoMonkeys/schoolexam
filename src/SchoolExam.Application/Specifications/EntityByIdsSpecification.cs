using SchoolExam.Domain.Base;

namespace SchoolExam.Application.Specifications;

public class EntityByIdsSpecification<TEntity> : EntitySpecification<TEntity>
    where TEntity : class, IEntity
{
    public EntityByIdsSpecification(HashSet<Guid> ids) : base(x => ids.Contains(x.Id))
    {
    }
}