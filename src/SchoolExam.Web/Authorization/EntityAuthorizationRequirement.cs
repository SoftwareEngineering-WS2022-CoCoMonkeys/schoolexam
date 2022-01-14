using Microsoft.AspNetCore.Authorization;

namespace SchoolExam.Web.Authorization;

public interface IEntityAuthorizationRequirement : IAuthorizationRequirement
{
    string ParameterName { get; }
}