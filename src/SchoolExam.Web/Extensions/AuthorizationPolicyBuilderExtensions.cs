using Microsoft.AspNetCore.Authorization;

namespace SchoolExam.Web.Extensions;

public static class AuthorizationPolicyBuilderExtensions
{
    public static AuthorizationPolicyBuilder AddRequirement<TRequirement>(this AuthorizationPolicyBuilder builder)
        where TRequirement : IAuthorizationRequirement, new()
    {
        var requirement = new TRequirement();
        builder.AddRequirements(requirement);
        return builder;
    }
}