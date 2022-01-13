using System.Security.Claims;
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
                var personIdString = httpContext.User.GetClaim(CustomClaimTypes.PersonId);
                if (personIdString == null)
                {
                    return Task.CompletedTask;
                }

                var personId = Guid.Parse(personIdString);
                var role = httpContext.User.GetClaim(ClaimTypes.Role)!;
                if (IsAuthorized(personId, role, entityId))
                {
                    context.Succeed(requirement);
                }
            }
        }

        return Task.CompletedTask;
    }

    protected abstract bool IsAuthorized(Guid personId, string role, Guid entityId);
}