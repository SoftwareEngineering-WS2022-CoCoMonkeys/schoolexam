using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolExam.Application.Authentication;
using SchoolExam.Application.Repositories;

namespace SchoolExam.Web.Authentication;

public class AuthenticationController : ApiController<AuthenticationController>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IPasswordHasher _passwordHasher; 

    public AuthenticationController(ILogger<AuthenticationController> logger, IUserRepository userRepository,
        ITokenGenerator tokenGenerator, IPasswordHasher passwordHasher) : base(logger)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
        _passwordHasher = passwordHasher;
    }

    [AllowAnonymous]
    [HttpPost("Authenticate")]
    public IActionResult Authenticate([FromBody] AuthenticateModel model)
    {
        var user = _userRepository.GetByUsername(model.Username);
        if (user == null)
            return BadRequest();

        if (!_passwordHasher.VerifyPassword(model.Password, user.Password))
            return BadRequest();

        var token = _tokenGenerator.Generate(user);

        return Ok(token);
    }
}