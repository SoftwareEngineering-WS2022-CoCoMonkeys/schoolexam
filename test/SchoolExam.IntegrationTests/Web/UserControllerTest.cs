using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SchoolExam.Domain.Entities.UserAggregate;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Infrastructure.Extensions;
using SchoolExam.IntegrationTests.Util;
using SchoolExam.IntegrationTests.Util.Extensions;
using SchoolExam.Web.Models.User;

namespace SchoolExam.IntegrationTests.Web;

[TestFixture]
public class UserControllerTest : ApiIntegrationTestBase
{
    private User _user = null!, _otherUser = null!;

    protected override async void SetUpData()
    {
        _user = TestEntityFactory.Create<User>();
        _otherUser = TestEntityFactory.Create<User>();

        using var repository = GetSchoolExamRepository();
        repository.Add(_user);
        repository.Add(_otherUser);
        await repository.SaveChangesAsync();
    }
    
    [Test]
    public async Task UserController_GetUserByUsername_Administrator_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Administrator));

        var response = await Client.GetAsync($"/User/{_user.Username}");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        var userResult = JsonConvert.DeserializeObject<UserReadModel>(result);

        var expectedUser = new UserReadModel
        {
            Id = _user.Id, UserName = _user.Username, Role = _user.Role, PersonId = _user.PersonId
        };
        userResult.Should().BeEquivalentTo(expectedUser);
    }
    
    [Test]
    public async Task UserController_GetUserById_Administrator_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Administrator));

        var response = await Client.GetAsync($"/User/ById/{_user.Id}");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        var userResult = JsonConvert.DeserializeObject<UserReadModel>(result);

        var expectedUser = new UserReadModel
        {
            Id = _user.Id, UserName = _user.Username, Role = _user.Role, PersonId = _user.PersonId
        };
        userResult.Should().BeEquivalentTo(expectedUser);
    }
    
    [Test]
    public async Task UserController_GetAllUsers_Administrator_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Administrator));

        var response = await Client.GetAsync("/User/GetAllUsers");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        var usersResult = JsonConvert.DeserializeObject<IEnumerable<UserReadModel>>(result).ToList();

        var expectedUser = new UserReadModel
        {
            Id = _user.Id, UserName = _user.Username, Role = _user.Role, PersonId = _user.PersonId
        };
        var expectedOtherUser = new UserReadModel
        {
            Id = _otherUser.Id, UserName = _otherUser.Username, Role = _otherUser.Role, PersonId = _otherUser.PersonId
        };
        
        usersResult.Should().HaveCount(2);
        usersResult.Should().ContainEquivalentOf(expectedUser);
        usersResult.Should().ContainEquivalentOf(expectedOtherUser);
    }
    
    [Test]
    public async Task UserController_Create_Administrator_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Administrator));

        var newUser = TestEntityFactory.Create<User>();
        var userWriteModel = new UserWriteModel
        {
            Username = newUser.Username, Password = newUser.Password, Role = newUser.Role,
            PersonId = newUser.PersonId
        };

        var response = await Client.PostAsJsonAsync("/User/Create", userWriteModel);
        response.EnsureSuccessStatusCode();

        using var repository = GetSchoolExamRepository();
        var users = repository.List<User>().ToList();
        users.Should().HaveCount(3);
        users.Should().ContainEquivalentOf(newUser, opt => opt.Excluding(x => x.Id));
        users.Should().ContainEquivalentOf(_user);
        users.Should().ContainEquivalentOf(_otherUser);
    }

    [Test]
    public async Task UserController_Update_Administrator_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Administrator));

        var updatedUser = TestEntityFactory.Create<User>();
        var userWriteModel = new UserWriteModel
        {
            Username = updatedUser.Username, Password = updatedUser.Password, Role = updatedUser.Role,
            PersonId = updatedUser.PersonId
        };

        var response = await Client.PutAsJsonAsync($"/User/{_user.Username}/Update", userWriteModel);
        response.EnsureSuccessStatusCode();

        using var repository = GetSchoolExamRepository();
        var users = repository.List<User>().ToList();
        users.Should().HaveCount(2);
        users.Should().ContainEquivalentOf(updatedUser, opt => opt.Excluding(x => x.Id));
        users.Should().ContainEquivalentOf(_otherUser);
    }
    
    [Test]
    public async Task UserController_Delete_Administrator_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Administrator));

        var response = await Client.DeleteAsync($"/User/{_user.Username}/Delete");
        response.EnsureSuccessStatusCode();

        using var repository = GetSchoolExamRepository();
        var users = repository.List<User>().ToList();
        users.Should().HaveCount(1);
        users.Should().ContainEquivalentOf(_otherUser);
    }
    
    [Test]
    public async Task UserController_Delete_UserDoesNotExist_ThrowsException()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Administrator));

        var username = "username";

        var response = await Client.DeleteAsync($"/User/{username}/Delete");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("User does not exist");
    }
}