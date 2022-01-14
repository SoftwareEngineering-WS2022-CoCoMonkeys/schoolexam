using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolExam.Application.Services;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Web.Authorization;
using SchoolExam.Web.Models.User;

namespace SchoolExam.Web.Controllers;

public class PersonController : ApiController<PersonController>
{
    private readonly IPersonService _personService;

    public PersonController(ILogger<PersonController> logger, IMapper mapper, IPersonService personService) :
        base(logger, mapper)
    {
        _personService = personService;
    }


    [HttpGet]
    [Route($"{{{RouteParameterNames.PersonIdParameterName}}}")]
    [Authorize]
    public PersonReadModelBase GetPersonById(Guid id)
    {
        var person = _personService.GetById(id);
        return Mapper.Map<PersonReadModelBase>(person);
    }

    
    [HttpPost]
    [Route("Create")]
    [Authorize(Roles = Role.AdministratorName)]
    public async Task<IActionResult> Create([FromBody] PersonWriteModel personWriteModel)
    {
        await _personService.Create(personWriteModel.FirstName,  personWriteModel.LastName, personWriteModel.DateOfBirth, 
            personWriteModel.Address, personWriteModel.EmailAddress);
        return Ok();
    }
    
    [HttpPost]
    [Route("CreateWithUser")]
    [Authorize(Roles = Role.AdministratorName)]
    public async Task<IActionResult> CreateWithUser([FromBody] PersonWriteWithUserModel personWriteWithUserModel)
    {
        await _personService.CreateWithUser(personWriteWithUserModel.FirstName,  personWriteWithUserModel.LastName, personWriteWithUserModel.DateOfBirth, 
            personWriteWithUserModel.Address, personWriteWithUserModel.EmailAddress, personWriteWithUserModel.Username, 
            personWriteWithUserModel.Password, personWriteWithUserModel.Role);
        return Ok();
    }
    
    [HttpPut]
    [Route($"{{{RouteParameterNames.PersonIdParameterName}}}/Update")]
    [Authorize(Roles = Role.AdministratorName)]
    public async Task<IActionResult> Update(Guid id, [FromBody] PersonWriteModel personWriteModel)
    {
        await _personService.Update(id,personWriteModel.FirstName,  personWriteModel.LastName, personWriteModel.DateOfBirth, 
            personWriteModel.Address, personWriteModel.EmailAddress);
        return Ok();
    }

    [HttpDelete]
    [Route($"{{{RouteParameterNames.PersonIdParameterName}}}/Delete")]
    [Authorize(Roles = Role.AdministratorName)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _personService.Delete(id);
        return Ok();
    }
}