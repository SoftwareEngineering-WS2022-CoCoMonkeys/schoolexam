using Microsoft.AspNetCore.Authorization;

namespace SchoolExam.Web.Authorization;

public interface IRouteParameterEntityAuthorizationRequirement : IAuthorizationRequirement
{
    string? ParameterName { get; }
}