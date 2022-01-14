namespace SchoolExam.Web.Authorization;

public abstract class RequestBodyEntityAuthorizationHandler<TRequirement> : EntityAuthorizationHandler<TRequirement>
    where TRequirement : IRequestBodyEntityAuthorizationRequirement
{
    protected override async Task<bool> IsAuthorized(Guid personId, string role, RouteValueDictionary routeParameters,
        HttpRequest? request,
        TRequirement requirement)
    {
        var body = await request!.ReadFromJsonAsync(requirement.Type);
        // reset the request body stream position such that the next middleware can read it
        request.Body.Position = 0;
        var result = await IsAuthorized(personId, role, body);
        return result;
    }

    protected abstract Task<bool> IsAuthorized(Guid personId, string role, object? body);
}