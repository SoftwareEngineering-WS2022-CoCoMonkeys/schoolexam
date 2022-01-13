using AutoMapper;
using SchoolExam.Application.Repositories;

namespace SchoolExam.Web.Controllers;

public class UserController : ApiController<UserController>
{
    private readonly IUserRepository _userRepository;

    public UserController(ILogger<UserController> logger, IMapper mapper, IUserRepository userRepository) :
        base(logger, mapper)
    {
        _userRepository = userRepository;
    }
}