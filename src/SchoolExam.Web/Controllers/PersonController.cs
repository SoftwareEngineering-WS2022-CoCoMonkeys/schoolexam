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
    private readonly IUserService _userService;

    public PersonController(ILogger<PersonController> logger, IMapper mapper, IPersonService personService,
        IUserService userService) : base(logger, mapper)
    {
        _personService = personService;
        _userService = userService;
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
    [Route("CreateStudent")]
    [Authorize(Roles = Role.AdministratorName)]
    public async Task<IActionResult> Create([FromBody] StudentWriteModel studentWriteModel)
    {
        var student = await _personService.CreateStudent(studentWriteModel.FirstName, studentWriteModel.LastName,
            studentWriteModel.DateOfBirth, Mapper.Map<Address>(studentWriteModel.Address),
            studentWriteModel.EmailAddress, studentWriteModel.SchoolId);
        return Ok(Mapper.Map<PersonReadModel>(student));
    }

    [HttpPost]
    [Route("CreateTeacher")]
    [Authorize(Roles = Role.AdministratorName)]
    public async Task<IActionResult> Create([FromBody] TeacherWriteModel teacherWriteModel)
    {
        var student = await _personService.CreateTeacher(teacherWriteModel.FirstName, teacherWriteModel.LastName,
            teacherWriteModel.DateOfBirth, Mapper.Map<Address>(teacherWriteModel.Address),
            teacherWriteModel.EmailAddress, teacherWriteModel.SchoolId);
        return Ok(Mapper.Map<PersonReadModel>(student));
    }

    [HttpPost]
    [Route("CreateLegalGuardian")]
    [Authorize(Roles = Role.AdministratorName)]
    public async Task<IActionResult> Create([FromBody] LegalGuardianWriteModel legalGuardianWriteModel)
    {
        var student = await _personService.CreateLegalGuardian(legalGuardianWriteModel.FirstName,
            legalGuardianWriteModel.LastName, legalGuardianWriteModel.DateOfBirth,
            Mapper.Map<Address>(legalGuardianWriteModel.Address), legalGuardianWriteModel.EmailAddress);
        return Ok(Mapper.Map<PersonReadModel>(student));
    }

    [HttpPost]
    [Route("CreateStudentWithUser")]
    [Authorize(Roles = Role.AdministratorName)]
    public async Task<IActionResult> CreateStudentWithUser(
        [FromBody] StudentWithUserWriteModel personWithUserWriteModel)
    {
        var address = Mapper.Map<Address>(personWithUserWriteModel.Address);
        var student = await _personService.CreateStudent(personWithUserWriteModel.FirstName,
            personWithUserWriteModel.LastName, personWithUserWriteModel.DateOfBirth, address,
            personWithUserWriteModel.EmailAddress, personWithUserWriteModel.SchoolId);
        var user = await _userService.CreateFromPerson(student.Id, personWithUserWriteModel.Username,
            personWithUserWriteModel.Password);
        return Ok(Mapper.Map<PersonWithUserReadModel>(user));
    }
    
    [HttpPost]
    [Route("CreateTeacherWithUser")]
    [Authorize(Roles = Role.AdministratorName)]
    public async Task<IActionResult> CreateTeacherWithUser(
        [FromBody] TeacherWithUserWriteModel teacherWithUserWriteModel)
    {
        var address = Mapper.Map<Address>(teacherWithUserWriteModel.Address);
        var teacher = await _personService.CreateTeacher(teacherWithUserWriteModel.FirstName,
            teacherWithUserWriteModel.LastName, teacherWithUserWriteModel.DateOfBirth, address,
            teacherWithUserWriteModel.EmailAddress, teacherWithUserWriteModel.SchoolId);
        var user = await _userService.CreateFromPerson(teacher.Id, teacherWithUserWriteModel.Username,
            teacherWithUserWriteModel.Password);
        teacher.User = user;
        return Ok(Mapper.Map<PersonWithUserReadModel>(teacher));
    }
    
    [HttpPost]
    [Route("CreateLegalGuardianWithUser")]
    [Authorize(Roles = Role.AdministratorName)]
    public async Task<IActionResult> CreateLegalGuardianWithUser(
        [FromBody] LegalGuardianWithUserWriteModel legalGuardianWithUserWriteModel)
    {
        var address = Mapper.Map<Address>(legalGuardianWithUserWriteModel.Address);
        var legalGuardian = await _personService.CreateLegalGuardian(legalGuardianWithUserWriteModel.FirstName,
            legalGuardianWithUserWriteModel.LastName, legalGuardianWithUserWriteModel.DateOfBirth, address,
            legalGuardianWithUserWriteModel.EmailAddress);
        var user = await _userService.CreateFromPerson(legalGuardian.Id, legalGuardianWithUserWriteModel.Username,
            legalGuardianWithUserWriteModel.Password);
        legalGuardian.User = user;
        return Ok(Mapper.Map<PersonWithUserReadModel>(legalGuardian));
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
    public async Task<IActionResult> Delete(Guid personId)
    {
        await _personService.Delete(personId);
        return Ok();
    }
}