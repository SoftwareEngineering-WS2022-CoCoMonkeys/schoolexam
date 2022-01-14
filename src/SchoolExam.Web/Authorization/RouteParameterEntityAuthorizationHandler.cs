namespace SchoolExam.Web.Authorization;

public abstract class RouteParameterEntityAuthorizationHandler<TRequirement> : EntityAuthorizationHandler<TRequirement>
    where TRequirement : IRouteParameterEntityAuthorizationRequirement
{
    protected override async Task<bool> IsAuthorized(Guid personId, string role, RouteValueDictionary routeParameters,
        HttpRequest? request, TRequirement requirement)
    {
        var resourceParameter = routeParameters[requirement.ParameterName];

        if (resourceParameter is string entityIdString)
        {
            var entityId = Guid.Parse(entityIdString);
            var isAuthorized = await IsAuthorized(personId, role, entityId);
            if (isAuthorized)
            {
                return true;
            }
        }

        return false;
    }

    protected abstract Task<bool> IsAuthorized(Guid personId, string role, Guid entityId);
}