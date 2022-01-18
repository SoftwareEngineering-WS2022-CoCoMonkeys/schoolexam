using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolExam.Application.Services;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Web.Authorization;
using SchoolExam.Web.Models.Person;
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
    public PersonReadModel GetPersonById(Guid personId)
    {
        var person = _personService.GetById(personId);
        return Mapper.Map<PersonReadModel>(person);
    }

    [HttpGet]
    [Route("GetAllPersons")]
    [Authorize(Roles = Role.AdministratorName)]
    public List<PersonReadModel> GetAllPersons()
    {
        var persons = _personService.GetAllPersons();
        return Mapper.Map<List<PersonReadModel>>(persons);
    }

    [HttpPost]
    [Route("Create")]
    [Authorize(Roles = Role.AdministratorName)]
    public async Task<IActionResult> Create([FromBody] PersonWriteModel personWriteModel)
    {
        var person = await _personService.Create(personWriteModel.FirstName, personWriteModel.LastName,
            personWriteModel.DateOfBirth, Mapper.Map<Address>(personWriteModel.Address), personWriteModel.EmailAddress);
        return Ok(Mapper.Map<PersonReadModel>(person));
    }

    [HttpPost]
    [Route("CreateWithUser")]
    [Authorize(Roles = Role.AdministratorName)]
    public async Task<IActionResult> CreateWithUser([FromBody] PersonWithUserWriteModel personWithUserWriteModel)
    {
        var userWithPerson = await _personService.CreateWithUser(personWithUserWriteModel.FirstName,
            personWithUserWriteModel.LastName, personWithUserWriteModel.DateOfBirth,
            Mapper.Map<Address>(personWithUserWriteModel.Address), personWithUserWriteModel.EmailAddress,
            personWithUserWriteModel.Username, personWithUserWriteModel.Password,
            new Role(personWithUserWriteModel.Role));
        return Ok(Mapper.Map<UserWithPersonReadModel>(userWithPerson));
    }

    [HttpPut]
    [Route($"{{{RouteParameterNames.PersonIdParameterName}}}/Update")]
    [Authorize(Roles = Role.AdministratorName)]
    public async Task<IActionResult> Update(Guid personId, [FromBody] PersonWriteModel personWriteModel)
    {
        var person = await _personService.Update(personId, personWriteModel.FirstName, personWriteModel.LastName,
            personWriteModel.DateOfBirth, Mapper.Map<Address>(personWriteModel.Address), personWriteModel.EmailAddress);
        return Ok(Mapper.Map<PersonReadModel>(person));
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