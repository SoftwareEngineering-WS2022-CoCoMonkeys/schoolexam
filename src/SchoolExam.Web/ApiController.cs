using Microsoft.AspNetCore.Mvc;
using SchoolExam.Core.UserManagement.UserAggregate;
using SchoolExam.SharedKernel.Extensions;

namespace SchoolExam.Web;

[ApiController]
[Route("[controller]")]
public class ApiController<TController> : ControllerBase where TController : ApiController<TController>
{
    protected ILogger<TController> Logger { get; private set; }

    public ApiController(ILogger<TController> logger)
    {
        Logger = logger;
    }

    protected string? GetUserId()
    {
        return User.GetClaim(CustomClaimTypes.UserId);
    }

    protected string? GetPersonId()
    {
        return User.GetClaim(CustomClaimTypes.PersonId);
    }
}