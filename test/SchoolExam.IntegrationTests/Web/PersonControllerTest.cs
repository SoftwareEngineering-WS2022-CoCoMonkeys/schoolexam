using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.Entities.SchoolAggregate;
using SchoolExam.Domain.Entities.UserAggregate;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Infrastructure.Extensions;
using SchoolExam.IntegrationTests.Util;
using SchoolExam.IntegrationTests.Util.Extensions;
using SchoolExam.Web.Models.Person;

namespace SchoolExam.IntegrationTests.Web;

[TestFixture]
public class PersonControllerTest : ApiIntegrationTestBase
{
    private School _school = null!;
    private Teacher _teacher = null!;
    private Student _student = null!;
    private LegalGuardian _legalGuardian = null!;

    protected override async void SetUpData()
    {
        _school = TestEntityFactory.Create<School>();
        _teacher = TestEntityFactory.Create<Teacher>();
        _teacher.SchoolId = _school.Id;
        _student = TestEntityFactory.Create<Student>();
        _student.SchoolId = _school.Id;
        _legalGuardian = TestEntityFactory.Create<LegalGuardian>();

        using var repository = GetSchoolExamRepository();
        repository.Add(_school);
        repository.Add(_teacher);
        repository.Add(_student);
        repository.Add(_legalGuardian);
        await repository.SaveChangesAsync();
    }

    [Test]
    public async Task CourseController_GetPersonById_Administrator_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Administrator));

        var response = await Client.GetAsync($"/Person/{_student.Id}");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        var personsResult = JsonConvert.DeserializeObject<PersonReadModel>(result);

        var expectedStudent = new PersonReadModel
        {
            Id = _student.Id, FirstName = _student.FirstName, LastName = _student.LastName,
            DateOfBirth = _student.DateOfBirth, EmailAddress = _student.EmailAddress,
            Address = new AddressReadModel
            {
                StreetName = _student.Address!.StreetName, StreetNumber = _student.Address.StreetNumber,
                PostCode = _student.Address.PostCode, City = _student.Address.City, Country = _student.Address.Country
            }
        };

        personsResult.Should().BeEquivalentTo(expectedStudent);
    }

    [Test]
    public async Task PersonController_GetAllPersons_Administrator_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Administrator));

        var response = await Client.GetAsync("/Person/GetAllPersons");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        var personsResult = JsonConvert.DeserializeObject<IEnumerable<PersonReadModel>>(result).ToList();

        var expectedStudent = new PersonReadModel
        {
            Id = _student.Id, FirstName = _student.FirstName, LastName = _student.LastName,
            DateOfBirth = _student.DateOfBirth, EmailAddress = _student.EmailAddress,
            Address = new AddressReadModel
            {
                StreetName = _student.Address!.StreetName, StreetNumber = _student.Address.StreetNumber,
                PostCode = _student.Address.PostCode, City = _student.Address.City, Country = _student.Address.Country
            }
        };
        var expectedTeacher = new PersonReadModel
        {
            Id = _teacher.Id, FirstName = _teacher.FirstName, LastName = _teacher.LastName,
            DateOfBirth = _teacher.DateOfBirth, EmailAddress = _teacher.EmailAddress,
            Address = new AddressReadModel
            {
                StreetName = _teacher.Address!.StreetName, StreetNumber = _teacher.Address.StreetNumber,
                PostCode = _teacher.Address.PostCode, City = _teacher.Address.City, Country = _teacher.Address.Country
            }
        };
        var expectedLegalGuardian = new PersonReadModel
        {
            Id = _legalGuardian.Id, FirstName = _legalGuardian.FirstName, LastName = _legalGuardian.LastName,
            DateOfBirth = _legalGuardian.DateOfBirth, EmailAddress = _legalGuardian.EmailAddress,
            Address = new AddressReadModel
            {
                StreetName = _legalGuardian.Address!.StreetName, StreetNumber = _legalGuardian.Address.StreetNumber,
                PostCode = _legalGuardian.Address.PostCode, City = _legalGuardian.Address.City,
                Country = _legalGuardian.Address.Country
            }
        };

        personsResult.Should().HaveCount(3);
        personsResult.Should().ContainEquivalentOf(expectedStudent);
        personsResult.Should().ContainEquivalentOf(expectedTeacher);
        personsResult.Should().ContainEquivalentOf(expectedLegalGuardian);
    }

    [Test]
    public async Task PersonController_CreateStudent_Administrator_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Administrator));

        var newStudent = TestEntityFactory.Create<Student>();
        newStudent.SchoolId = _school.Id;

        var studentWriteModel = new StudentWriteModel
        {
            FirstName = newStudent.FirstName, LastName = newStudent.LastName,
            DateOfBirth = newStudent.DateOfBirth, EmailAddress = newStudent.EmailAddress,
            SchoolId = newStudent.SchoolId, Address = new AddressWriteModel
            {
                StreetName = newStudent.Address!.StreetName, StreetNumber = newStudent.Address.StreetNumber,
                PostCode = newStudent.Address.PostCode, City = newStudent.Address.City,
                Country = newStudent.Address.Country
            }
        };
        var response = await Client.PostAsJsonAsync("/Person/CreateStudent", studentWriteModel);
        response.EnsureSuccessStatusCode();

        using var repository = GetSchoolExamRepository();
        var persons = repository.List<Person>().ToList();
        persons.Should().HaveCount(4);
        var teachers = repository.List<Teacher>().ToList();
        teachers.Should().HaveCount(1);
        teachers.Should().ContainEquivalentOf(_teacher, opt => opt.Excluding(x => x.Courses));
        var students = repository.List<Student>().ToList();
        students.Should().HaveCount(2);
        students.Should()
            .ContainEquivalentOf(_student, opt => opt.Excluding(x => x.Courses).Excluding(x => x.LegalGuardians));
        students.Should().ContainEquivalentOf(newStudent,
            opt => opt.Excluding(x => x.Id).Excluding(x => x.Courses).Excluding(x => x.LegalGuardians)
                .Excluding(x => x.QrCode));
        var legalGuardians = repository.List<LegalGuardian>().ToList();
        legalGuardians.Should().HaveCount(1);
        legalGuardians.Should().ContainEquivalentOf(_legalGuardian, opt => opt.Excluding(x => x.Children));
    }

    [Test]
    public async Task PersonController_CreateStudentWithUser_Administrator_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Administrator));

        var newStudent = TestEntityFactory.Create<Student>();
        newStudent.SchoolId = _school.Id;
        var newUser = TestEntityFactory.Create<User>();
        newUser.Role = Role.Student;

        var studentWithUserWriteModel = new StudentWithUserWriteModel
        {
            FirstName = newStudent.FirstName, LastName = newStudent.LastName,
            DateOfBirth = newStudent.DateOfBirth, EmailAddress = newStudent.EmailAddress,
            SchoolId = newStudent.SchoolId, Username = newUser.Username, Password = newUser.Password,
            Address = new AddressWriteModel
            {
                StreetName = newStudent.Address!.StreetName, StreetNumber = newStudent.Address.StreetNumber,
                PostCode = newStudent.Address.PostCode, City = newStudent.Address.City,
                Country = newStudent.Address.Country
            }
        };
        var response = await Client.PostAsJsonAsync("/Person/CreateStudentWithUser", studentWithUserWriteModel);
        response.EnsureSuccessStatusCode();

        using var repository = GetSchoolExamRepository();
        var persons = repository.List<Person>().ToList();
        persons.Should().HaveCount(4);
        var teachers = repository.List<Teacher>().ToList();
        teachers.Should().HaveCount(1);
        teachers.Should().ContainEquivalentOf(_teacher, opt => opt.Excluding(x => x.Courses));
        var students = repository.List<Student>().ToList();
        students.Should().HaveCount(2);
        students.Should()
            .ContainEquivalentOf(_student, opt => opt.Excluding(x => x.Courses).Excluding(x => x.LegalGuardians));
        students.Should().ContainEquivalentOf(newStudent,
            opt => opt.Excluding(x => x.Id).Excluding(x => x.Courses).Excluding(x => x.LegalGuardians)
                .Excluding(x => x.QrCode));
        var legalGuardians = repository.List<LegalGuardian>().ToList();
        legalGuardians.Should().HaveCount(1);
        legalGuardians.Should().ContainEquivalentOf(_legalGuardian, opt => opt.Excluding(x => x.Children));

        var users = repository.List<User>().ToList();
        users.Should().HaveCount(1);
        users.Should().ContainEquivalentOf(newUser,
            opt => opt.Excluding(x => x.Id).Excluding(x => x.PersonId).Excluding(x => x.Person));
        var personId = users.Single().PersonId!.Value;
        students.Select(x => x.Id).Should().Contain(personId);
    }
    
    [Test]
    public async Task PersonController_CreateTeacherWithUser_Administrator_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Administrator));

        var newTeacher = TestEntityFactory.Create<Teacher>();
        newTeacher.SchoolId = _school.Id;
        var newUser = TestEntityFactory.Create<User>();
        newUser.Role = Role.Teacher;

        var teacherWithUserWriteModel = new TeacherWithUserWriteModel
        {
            FirstName = newTeacher.FirstName, LastName = newTeacher.LastName,
            DateOfBirth = newTeacher.DateOfBirth, EmailAddress = newTeacher.EmailAddress,
            SchoolId = newTeacher.SchoolId, Username = newUser.Username, Password = newUser.Password,
            Address = new AddressWriteModel
            {
                StreetName = newTeacher.Address!.StreetName, StreetNumber = newTeacher.Address.StreetNumber,
                PostCode = newTeacher.Address.PostCode, City = newTeacher.Address.City,
                Country = newTeacher.Address.Country
            }
        };
        var response = await Client.PostAsJsonAsync("/Person/CreateTeacherWithUser", teacherWithUserWriteModel);
        response.EnsureSuccessStatusCode();

        using var repository = GetSchoolExamRepository();
        var persons = repository.List<Person>().ToList();
        persons.Should().HaveCount(4);
        var teachers = repository.List<Teacher>().ToList();
        teachers.Should().HaveCount(2);
        teachers.Should().ContainEquivalentOf(_teacher, opt => opt.Excluding(x => x.Courses));
        teachers.Should().ContainEquivalentOf(newTeacher, opt => opt.Excluding(x => x.Id).Excluding(x => x.Courses));
        var students = repository.List<Student>().ToList();
        students.Should().HaveCount(1);
        students.Should()
            .ContainEquivalentOf(_student, opt => opt.Excluding(x => x.Courses).Excluding(x => x.LegalGuardians));
        var legalGuardians = repository.List<LegalGuardian>().ToList();
        legalGuardians.Should().HaveCount(1);
        legalGuardians.Should().ContainEquivalentOf(_legalGuardian, opt => opt.Excluding(x => x.Children));

        var users = repository.List<User>().ToList();
        users.Should().HaveCount(1);
        users.Should().ContainEquivalentOf(newUser,
            opt => opt.Excluding(x => x.Id).Excluding(x => x.PersonId).Excluding(x => x.Person));
        var personId = users.Single().PersonId!.Value;
        teachers.Select(x => x.Id).Should().Contain(personId);
    }
    
    [Test]
    public async Task PersonController_CreateLegalGuardianWithUser_Administrator_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Administrator));

        var newLegalGuardian = TestEntityFactory.Create<LegalGuardian>();
        var newUser = TestEntityFactory.Create<User>();
        newUser.Role = Role.LegalGuardian;

        var legalGuardianWithUserWriteModel = new LegalGuardianWithUserWriteModel()
        {
            FirstName = newLegalGuardian.FirstName, LastName = newLegalGuardian.LastName,
            DateOfBirth = newLegalGuardian.DateOfBirth, EmailAddress = newLegalGuardian.EmailAddress,
            Username = newUser.Username, Password = newUser.Password, Address = new AddressWriteModel
            {
                StreetName = newLegalGuardian.Address!.StreetName, StreetNumber = newLegalGuardian.Address.StreetNumber,
                PostCode = newLegalGuardian.Address.PostCode, City = newLegalGuardian.Address.City,
                Country = newLegalGuardian.Address.Country
            }
        };
        var response =
            await Client.PostAsJsonAsync("/Person/CreateLegalGuardianWithUser", legalGuardianWithUserWriteModel);
        response.EnsureSuccessStatusCode();

        using var repository = GetSchoolExamRepository();
        var persons = repository.List<Person>().ToList();
        persons.Should().HaveCount(4);
        var teachers = repository.List<Teacher>().ToList();
        teachers.Should().HaveCount(1);
        teachers.Should().ContainEquivalentOf(_teacher, opt => opt.Excluding(x => x.Courses));
        var students = repository.List<Student>().ToList();
        students.Should().HaveCount(1);
        students.Should()
            .ContainEquivalentOf(_student, opt => opt.Excluding(x => x.Courses).Excluding(x => x.LegalGuardians));
        var legalGuardians = repository.List<LegalGuardian>().ToList();
        legalGuardians.Should().HaveCount(2);
        legalGuardians.Should().ContainEquivalentOf(_legalGuardian, opt => opt.Excluding(x => x.Children));
        legalGuardians.Should().ContainEquivalentOf(newLegalGuardian, opt => opt.Excluding(x => x.Id).Excluding(x => x.Children));

        var users = repository.List<User>().ToList();
        users.Should().HaveCount(1);
        users.Should().ContainEquivalentOf(newUser,
            opt => opt.Excluding(x => x.Id).Excluding(x => x.PersonId).Excluding(x => x.Person));
        var personId = users.Single().PersonId!.Value;
        legalGuardians.Select(x => x.Id).Should().Contain(personId);
    }

    [Test]
    public async Task PersonController_CreateTeacher_Administrator_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Administrator));

        var newTeacher = TestEntityFactory.Create<Teacher>();
        newTeacher.SchoolId = _school.Id;

        var teacherWriteModel = new TeacherWriteModel
        {
            FirstName = newTeacher.FirstName, LastName = newTeacher.LastName,
            DateOfBirth = newTeacher.DateOfBirth, EmailAddress = newTeacher.EmailAddress,
            SchoolId = newTeacher.SchoolId, Address = new AddressWriteModel
            {
                StreetName = newTeacher.Address!.StreetName, StreetNumber = newTeacher.Address.StreetNumber,
                PostCode = newTeacher.Address.PostCode, City = newTeacher.Address.City,
                Country = newTeacher.Address.Country
            }
        };
        var response = await Client.PostAsJsonAsync("/Person/CreateTeacher", teacherWriteModel);
        response.EnsureSuccessStatusCode();

        using var repository = GetSchoolExamRepository();
        var persons = repository.List<Person>().ToList();
        persons.Should().HaveCount(4);
        var students = repository.List<Student>().ToList();
        students.Should().HaveCount(1);
        students.Single().Should()
            .BeEquivalentTo(_student, opt => opt.Excluding(x => x.Courses).Excluding(x => x.LegalGuardians));
        var teachers = repository.List<Teacher>().ToList();
        teachers.Should().HaveCount(2);
        teachers.Should().ContainEquivalentOf(_teacher, opt => opt.Excluding(x => x.Courses));
        teachers.Should().ContainEquivalentOf(newTeacher, opt => opt.Excluding(x => x.Id).Excluding(x => x.Courses));
        var legalGuardians = repository.List<LegalGuardian>().ToList();
        legalGuardians.Should().HaveCount(1);
        legalGuardians.Should().ContainEquivalentOf(_legalGuardian, opt => opt.Excluding(x => x.Children));
    }

    [Test]
    public async Task PersonController_CreateLegalGuardian_Administrator_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Administrator));

        var newLegalGuardian = TestEntityFactory.Create<LegalGuardian>();

        var legalGuardianWriteModel = new LegalGuardianWriteModel
        {
            FirstName = newLegalGuardian.FirstName, LastName = newLegalGuardian.LastName,
            DateOfBirth = newLegalGuardian.DateOfBirth, EmailAddress = newLegalGuardian.EmailAddress,
            Address = new AddressWriteModel
            {
                StreetName = newLegalGuardian.Address!.StreetName, StreetNumber = newLegalGuardian.Address.StreetNumber,
                PostCode = newLegalGuardian.Address.PostCode, City = newLegalGuardian.Address.City,
                Country = newLegalGuardian.Address.Country
            }
        };
        var response = await Client.PostAsJsonAsync("/Person/CreateLegalGuardian", legalGuardianWriteModel);
        response.EnsureSuccessStatusCode();

        using var repository = GetSchoolExamRepository();
        var persons = repository.List<Person>().ToList();
        persons.Should().HaveCount(4);
        var students = repository.List<Student>().ToList();
        students.Should().HaveCount(1);
        students.Single().Should()
            .BeEquivalentTo(_student, opt => opt.Excluding(x => x.Courses).Excluding(x => x.LegalGuardians));
        var teachers = repository.List<Teacher>().ToList();
        teachers.Should().HaveCount(1);
        teachers.Should().ContainEquivalentOf(_teacher, opt => opt.Excluding(x => x.Courses));
        var legalGuardians = repository.List<LegalGuardian>().ToList();
        legalGuardians.Should().HaveCount(2);
        legalGuardians.Should().ContainEquivalentOf(_legalGuardian, opt => opt.Excluding(x => x.Children));
        legalGuardians.Should()
            .ContainEquivalentOf(newLegalGuardian, opt => opt.Excluding(x => x.Id).Excluding(x => x.Children));
    }

    [Test]
    public async Task PersonController_Update_Administrator_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Administrator));

        var updatedPerson = TestEntityFactory.Create<Teacher>();
        updatedPerson.SchoolId = _school.Id;
        updatedPerson.Id = _teacher.Id;

        var personWriteModel = new PersonWriteModel
        {
            FirstName = updatedPerson.FirstName, LastName = updatedPerson.LastName,
            DateOfBirth = updatedPerson.DateOfBirth, EmailAddress = updatedPerson.EmailAddress,
            Address = new AddressWriteModel
            {
                StreetName = updatedPerson.Address!.StreetName, StreetNumber = updatedPerson.Address.StreetNumber,
                PostCode = updatedPerson.Address.PostCode, City = updatedPerson.Address.City,
                Country = updatedPerson.Address.Country
            }
        };
        var response = await Client.PutAsJsonAsync($"/Person/{_teacher.Id}/Update", personWriteModel);
        response.EnsureSuccessStatusCode();

        using var repository = GetSchoolExamRepository();
        var persons = repository.List<Person>().ToList();
        persons.Should().HaveCount(3);
        var teachers = repository.List<Teacher>().ToList();
        teachers.Should().HaveCount(1);
        teachers.Single().Should().BeEquivalentTo(updatedPerson, opt => opt.Excluding(x => x.Courses));
        var students = repository.List<Student>().ToList();
        students.Should().HaveCount(1);
        students.Single().Should()
            .BeEquivalentTo(_student, opt => opt.Excluding(x => x.Courses).Excluding(x => x.LegalGuardians));
        var legalGuardians = repository.List<LegalGuardian>().ToList();
        legalGuardians.Should().HaveCount(1);
        legalGuardians.Should().ContainEquivalentOf(_legalGuardian, opt => opt.Excluding(x => x.Children));
    }

    [Test]
    public async Task PersonController_Delete_Administrator_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Administrator));

        var response = await Client.DeleteAsync($"/Person/{_teacher.Id}/Delete");
        response.EnsureSuccessStatusCode();

        using var repository = GetSchoolExamRepository();
        var persons = repository.List<Person>().ToList();
        persons.Should().HaveCount(2);
        var teachers = repository.List<Teacher>().ToList();
        teachers.Should().HaveCount(0);
        var students = repository.List<Student>().ToList();
        students.Should().HaveCount(1);
        students.Single().Should()
            .BeEquivalentTo(_student, opt => opt.Excluding(x => x.Courses).Excluding(x => x.LegalGuardians));
        var legalGuardians = repository.List<LegalGuardian>().ToList();
        legalGuardians.Should().HaveCount(1);
        legalGuardians.Should().ContainEquivalentOf(_legalGuardian, opt => opt.Excluding(x => x.Children));
    }

    [Test]
    public async Task PersonController_Delete_PersonDoesNotExist_ThrowsException()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Administrator));

        var response = await Client.DeleteAsync($"/Person/{Guid.NewGuid()}/Delete");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Person does not exist.");

        using var repository = GetSchoolExamRepository();
        var persons = repository.List<Person>().ToList();
        persons.Should().HaveCount(3);
        var teachers = repository.List<Teacher>().ToList();
        teachers.Should().HaveCount(1);
        teachers.Single().Should().BeEquivalentTo(_teacher, opt => opt.Excluding(x => x.Courses));
        var students = repository.List<Student>().ToList();
        students.Should().HaveCount(1);
        students.Single().Should()
            .BeEquivalentTo(_student, opt => opt.Excluding(x => x.Courses).Excluding(x => x.LegalGuardians));
        var legalGuardians = repository.List<LegalGuardian>().ToList();
        legalGuardians.Should().HaveCount(1);
        legalGuardians.Should().ContainEquivalentOf(_legalGuardian, opt => opt.Excluding(x => x.Children));
    }
}