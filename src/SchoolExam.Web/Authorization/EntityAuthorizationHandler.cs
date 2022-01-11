using Microsoft.AspNetCore.Authorization;
using SchoolExam.Infrastructure.Authentication;
using SchoolExam.Web.Extensions;

namespace SchoolExam.Web.Authorization;

public abstract class EntityAuthorizationHandler<TRequirement> : AuthorizationHandler<TRequirement>
    where TRequirement : IEntityAuthorizationRequirement
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement)
    {
        if (context.Resource is DefaultHttpContext httpContext)
        {
            var resourceParameter = httpContext.GetRouteData().Values[requirement.ParameterName];
            if (resourceParameter is string entityIdString)
            {
                var entityId = Guid.Parse(entityIdString);
                var personId = Guid.Parse(context.User.GetClaim(CustomClaimTypes.PersonId)!);
                if (IsAuthorized(personId, entityId))
                {
                    context.Succeed(requirement);
                }
            }
        }

        return Task.CompletedTask;
    }

    protected abstract bool IsAuthorized(Guid personId, Guid entityId);
}