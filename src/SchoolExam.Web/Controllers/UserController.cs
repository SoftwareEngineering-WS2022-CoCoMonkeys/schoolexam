﻿using AutoMapper;
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
    public UserReadModel GetUserByUsername(String username)
    {
        var user = _userService.GetByUsername(username);
        return Mapper.Map<UserReadModel>(user);
    }
    
    [HttpGet]
    [Route($"{{{RouteParameterNames.UserIdParameterName}}}")]
    [Authorize(Roles = Role.AdministratorName)]
    public UserReadModel GetUserById(Guid id)
    {
        var user = _userService.GetById(id);
        return Mapper.Map<UserReadModel>(user);
    }
    
    [HttpPost]
    [Route("Create")]
    [Authorize(Roles = Role.AdministratorName)]
    public async Task<IActionResult> Create(string username,  [FromBody] UserWriteModel userWriteModel)
    {
        var personId = !string.IsNullOrEmpty(userWriteModel.PersonId) ? (Guid?) Guid.Parse(userWriteModel.PersonId) : null;
        await _userService.Create(username, userWriteModel.Password, userWriteModel.Role, personId);
        return Ok();
    }
    
    [HttpPut]
    [Route($"{{{RouteParameterNames.UserNameParameterName}}}/Update")]
    [Authorize(Roles = Role.AdministratorName)]
    public async Task<IActionResult> Update(string username, [FromBody] UserWriteModel userWriteModel)
    {
        
        var personId = !string.IsNullOrEmpty(userWriteModel.PersonId) ? (Guid?) Guid.Parse(userWriteModel.PersonId) : null;
        await _userService.Update( username, userWriteModel.Password, userWriteModel.Role, personId);
        return Ok();
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