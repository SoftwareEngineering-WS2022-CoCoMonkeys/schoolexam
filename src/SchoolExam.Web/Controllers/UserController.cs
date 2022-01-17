using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SchoolExam.Application.Services;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Web.Authorization;
using SchoolExam.Web.Models.Exam;
using SchoolExam.Web.Models.User;

namespace SchoolExam.Web.Controllers;

public class UserController : ApiController<UserController>
{
    private readonly IUserService _userService;

    public UserController(ILogger<UserController> logger, IMapper mapper, IUserService userService) :
        base(logger, mapper)
    {
        _userService = userService;
    }
    
    [HttpGet]
    [Route($"{{{RouteParameterNames.UserNameParameterName}}}")]
    [Authorize(Roles = Role.AdministratorName)]
    public UserReadModel GetUserByUsername(String userName)
    {
        var user = _userService.GetByUsername(userName);
        return Mapper.Map<UserReadModel>(user);
    }
    
    [HttpGet]
    [Route($"ById/{{{RouteParameterNames.UserIdParameterName}}}")]
    [Authorize(Roles = Role.AdministratorName)]
    public UserReadModel GetUserById(Guid userId)
    {
        var user = _userService.GetById(userId);
        return Mapper.Map<UserReadModel>(user);
    }
    
    [HttpGet]
    [Route("GetAllUsers")]
    [Authorize(Roles = Role.AdministratorName)]
    public List<UserReadModel> GetAllUser()
    {
        var users = _userService.GetAllUsers();
        return Mapper.Map<List<UserReadModel>>(users);
    }
    
    
    [HttpPost]
    [Route($"Create")]
    [Authorize(Roles = Role.AdministratorName)]
    public async Task<IActionResult> Create([FromBody] UserWriteModel userWriteModel)
    {
        var personId = !string.IsNullOrEmpty(userWriteModel.PersonId) ? (Guid?) Guid.Parse(userWriteModel.PersonId) : null;
        var user = await _userService.Create(userWriteModel.Username, userWriteModel.Password, new Role(userWriteModel.Role), personId);
        return Ok(Mapper.Map<UserReadModel>(user));
    }
    
    [HttpPut]
    [Route($"{{{RouteParameterNames.UserNameParameterName}}}/Update")]
    [Authorize(Roles = Role.AdministratorName)]
    public async Task<IActionResult> Update(string userName, [FromBody] UserWriteModel userWriteModel)
    {
        
        var personId = !string.IsNullOrEmpty(userWriteModel.PersonId) ? (Guid?) Guid.Parse(userWriteModel.PersonId) : null;
        var user = await _userService.Update( userName, userWriteModel.Password, Mapper.Map<Role>(userWriteModel.Role), personId);
        return Ok(Mapper.Map<UserReadModel>(user));
    }

    [HttpDelete]
    [Route($"{{{RouteParameterNames.UserNameParameterName}}}/Delete")]
    [Authorize(Roles = Role.AdministratorName)]
    public async Task<IActionResult> Delete(String userName)
    {
        await _userService.Delete(userName);
        return Ok();
    }

}