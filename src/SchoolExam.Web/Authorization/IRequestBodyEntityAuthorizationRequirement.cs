using Microsoft.AspNetCore.Authorization;

namespace SchoolExam.Web.Authorization;

public interface IRequestBodyEntityAuthorizationRequirement : IAuthorizationRequirement
{
    Type Type { get; }
}