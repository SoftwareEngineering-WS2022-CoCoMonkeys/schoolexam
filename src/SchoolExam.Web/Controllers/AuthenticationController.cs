using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolExam.Application.Authentication;
using SchoolExam.Application.Services;
using SchoolExam.Web.Models.Authentication;

namespace SchoolExam.Web.Controllers;

public class AuthenticationController : ApiController<AuthenticationController>
{
    private readonly IUserService _userService;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IPasswordHasher _passwordHasher;

    public AuthenticationController(ILogger<AuthenticationController> logger, IMapper mapper,
        IUserService userService, ITokenGenerator tokenGenerator, IPasswordHasher passwordHasher) : base(logger,
        mapper)
    {
        _userService = userService;
        _tokenGenerator = tokenGenerator;
        _passwordHasher = passwordHasher;
    }

    [AllowAnonymous]
    [HttpPost("Authenticate")]
    public IActionResult Authenticate([FromBody] AuthenticateModel model)
    {
        var user = _userService.GetByUsername(model.Username);
        if (user == null)
            return Forbid();

        if (!_passwordHasher.VerifyPassword(model.Password, user.Password))
            return Forbid();

        var token = _tokenGenerator.Generate(user);

        var authenticatedUser = Mapper.Map<AuthenticatedUserModel>(user);
        var result = new AuthenticationResultModel {Token = token, User = authenticatedUser};
        
        return Ok(result);
    }
}