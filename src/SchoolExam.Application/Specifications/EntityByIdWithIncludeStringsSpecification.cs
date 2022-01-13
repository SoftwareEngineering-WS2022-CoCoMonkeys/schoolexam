using SchoolExam.Domain.Base;

namespace SchoolExam.Application.Specifications;

public class EntityByIdWithIncludeStringsSpecification<TEntity, TIdentity> : EntityByIdSpecification<TEntity, TIdentity>
    where TEntity : class, IEntity<TIdentity>
{
    public EntityByIdWithIncludeStringsSpecification(TIdentity id, params string[] includes) : base(id)
    {
        foreach (var include in includes)
        {
            AddInclude(include);
        }
    }
}