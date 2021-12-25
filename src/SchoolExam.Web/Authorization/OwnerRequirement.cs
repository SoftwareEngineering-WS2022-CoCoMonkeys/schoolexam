using Microsoft.AspNetCore.Authorization;
using SchoolExam.Domain.Base;

namespace SchoolExam.Web.Authorization;

public class OwnerRequirement<TEntity> : IAuthorizationRequirement where TEntity : IEntity<Guid>
{
    public Func<TEntity, IEnumerable<Guid>> GetOwnerIds { get; }
    public string ParameterName { get; }
    public IEnumerable<string> Includes { get; }

    public OwnerRequirement(Func<TEntity, Guid> getOwnerId, string parameterName, params string[] includes) : this(
        entity => new[] {getOwnerId(entity)}, parameterName, includes)
    {
    }

    public OwnerRequirement(Func<TEntity, IEnumerable<Guid>> getOwnerIds, string parameterName,
        params string[] includes)
    {
        GetOwnerIds = getOwnerIds;
        ParameterName = parameterName;
        Includes = includes;
    }
}