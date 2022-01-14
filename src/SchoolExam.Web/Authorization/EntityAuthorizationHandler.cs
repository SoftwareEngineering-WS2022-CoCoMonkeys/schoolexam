using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using SchoolExam.Infrastructure.Authentication;
using SchoolExam.Web.Extensions;

namespace SchoolExam.Web.Authorization;

public abstract class EntityAuthorizationHandler<TRequirement> : AuthorizationHandler<TRequirement>
    where TRequirement : IAuthorizationRequirement
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement)
    {
        if (context.Resource is DefaultHttpContext httpContext)
        {
            var routeParameters = httpContext.GetRouteData().Values;
            var request = httpContext.Request;
            request.EnableBuffering();
            var personIdString = httpContext.User.GetClaim(CustomClaimTypes.PersonId);
            if (personIdString == null)
            {
                return;
            }
            var personId = Guid.Parse(personIdString);
            // rule has a value if claim for PersonId is set
            var role = httpContext.User.GetClaim(ClaimTypes.Role)!;

            var isAuthorized = await IsAuthorized(personId, role, routeParameters, request, requirement);
            if (isAuthorized)
            {
                context.Succeed(requirement);
            }
        }
    }

    protected abstract Task<bool> IsAuthorized(Guid personId, string role, RouteValueDictionary routeParameters,
        HttpRequest? request, TRequirement requirement);
}