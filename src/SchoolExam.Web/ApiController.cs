using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SchoolExam.Infrastructure.Authentication;
using SchoolExam.Web.Extensions;

namespace SchoolExam.Web;

[ApiController]
[Route("[controller]")]
public abstract class ApiController<TController> : ControllerBase where TController : ApiController<TController>
{
    protected ILogger<TController> Logger { get; private set; }
    protected IMapper Mapper { get; private set; }

    protected ApiController(ILogger<TController> logger, IMapper mapper)
    {
        Logger = logger;
        Mapper = mapper;
    }

    protected Guid? GetUserId()
    {
        var userId = User.GetClaim(CustomClaimTypes.UserId);
        return userId != null ? Guid.Parse(userId) : null;
    }

    protected Guid? GetPersonId()
    {
        var personId = User.GetClaim(CustomClaimTypes.PersonId);
        return personId != null ? Guid.Parse(personId) : null;
    }
}