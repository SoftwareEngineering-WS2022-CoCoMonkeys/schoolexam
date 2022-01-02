using Microsoft.AspNetCore.Authorization;
using SchoolExam.Application.DataContext;
using SchoolExam.Domain.Base;
using SchoolExam.Infrastructure.Authentication;
using SchoolExam.Web.Extensions;

namespace SchoolExam.Web.Authorization;

public class OwnerHandler<TEntity> : AuthorizationHandler<OwnerRequirement<TEntity>>
    where TEntity : class, IEntity<Guid>
{
    private readonly ISchoolExamDataContext _dataContext;

    public OwnerHandler(ISchoolExamDataContext dataContext)
    {
        _dataContext = dataContext;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        OwnerRequirement<TEntity> requirement)
    {
        if (context.Resource is DefaultHttpContext httpContext)
        {
            var resourceParameter = httpContext.GetRouteData().Values[requirement.ParameterName];
            if (resourceParameter is string resourceId)
            {
                var entity = _dataContext.Find<TEntity, Guid>(Guid.Parse(resourceId), requirement.Includes.ToArray());
                if (entity != null)
                {
                    var ownerIds = requirement.GetOwnerIds(entity).Select(x => x.ToString()).ToHashSet();
                    var personId = context.User.GetClaim(CustomClaimTypes.PersonId);
                    if (personId != null && ownerIds.Contains(personId))
                    {
                        context.Succeed(requirement);
                    }
                }
                else
                {
                    // authorization successful if entity does not exist
                    context.Succeed(requirement);
                }
            }
        }

        return Task.CompletedTask;
    }
}