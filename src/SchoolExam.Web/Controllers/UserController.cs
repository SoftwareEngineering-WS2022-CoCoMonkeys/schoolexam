using AutoMapper;
using SchoolExam.Application.Services;

namespace SchoolExam.Web.Controllers;

public class UserController : ApiController<UserController>
{
    private readonly IUserService _userService;

    public UserController(ILogger<UserController> logger, IMapper mapper, IUserService userService) :
        base(logger, mapper)
    {
        _userService = userService;
    }
}