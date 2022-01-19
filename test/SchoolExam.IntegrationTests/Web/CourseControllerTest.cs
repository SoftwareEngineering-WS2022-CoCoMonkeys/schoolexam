using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.Entities.SchoolAggregate;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Infrastructure.Authentication;
using SchoolExam.Infrastructure.Extensions;
using SchoolExam.Infrastructure.Specifications;
using SchoolExam.IntegrationTests.Util;
using SchoolExam.Web.Models.Course;

namespace SchoolExam.IntegrationTests.Web;

[TestFixture]
public class CourseControllerTest : ApiIntegrationTestBase
{
    private School _school = null!;
    private Course _course = null!;
    private Teacher _teacher = null!;
    private Student _student = null!;

    protected override async void SetUpData()
    {
        _school = TestEntityFactory.Create<School>();
        _course = TestEntityFactory.Create<Course>();
        _course.SchoolId = _school.Id;
        _teacher = TestEntityFactory.Create<Teacher>();
        _teacher.SchoolId = _school.Id;
        var courseTeacher = new CourseTeacher(_course.Id, _teacher.Id);
        _student = TestEntityFactory.Create<Student>();
        _student.SchoolId = _school.Id;
        var courseStudent = new CourseStudent(_course.Id, _student.Id);

        using var repository = GetSchoolExamRepository();
        repository.Add(_school);
        repository.Add(_course);
        repository.Add(_teacher);
        repository.Add(courseTeacher);
        repository.Add(_student);
        repository.Add(courseStudent);
        await repository.SaveChangesAsync();
    }

    [Test]
    public async Task CourseController_GetByIdTeacherView_CourseTeacher_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()));

        var response = await this.Client.GetAsync($"/Course/{_course.Id}/TeacherView");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        var courseResult = JsonConvert.DeserializeObject<CourseReadModelTeacher?>(result);

        courseResult.Should().NotBeNull();

        var expectedCourse = new CourseReadModelTeacher
        {
            Id = _course.Id.ToString(), Name = _course.Name,
            Topic = _course.Topic?.Name, Students = new[]
            {
                new CourseStudentReadModel
                {
                    Id = _student.Id, FirstName = _student.FirstName, LastName = _student.LastName,
                    EmailAddress = _student.EmailAddress, DateOfBirth = _student.DateOfBirth
                }
            }
        };
        courseResult.Should().BeEquivalentTo(expectedCourse);
    }

    [Test]
    public async Task CourseController_GetByIdTeacherView_NoCourseTeacher_Unauthorized()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, Guid.NewGuid().ToString()));

        var response = await this.Client.GetAsync($"/Course/{_course.Id}/TeacherView");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task CourseController_GetByIdStudentView_CourseStudent_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Student),
            new Claim(CustomClaimTypes.PersonId, _student.Id.ToString()));

        var response = await this.Client.GetAsync($"/Course/{_course.Id}/StudentView");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        var courseResult = JsonConvert.DeserializeObject<CourseReadModelStudent?>(result);

        courseResult.Should().NotBeNull();

        var expectedCourseDto = new CourseReadModelStudent
        {
            Id = _course.Id.ToString(), Name = _course.Name,
            Topic = _course.Topic?.Name
        };
        courseResult.Should().BeEquivalentTo(expectedCourseDto);
    }

    [Test]
    public async Task CourseController_GetByIdStudentView_NoCourseStudent_Unauthorized()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Student),
            new Claim(CustomClaimTypes.PersonId, Guid.NewGuid().ToString()));

        var response = await this.Client.GetAsync($"/Course/{_course.Id}/StudentView");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Test]
    public async Task CourseController_Create_Teacher_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()));

        var newCourse = TestEntityFactory.Create<Course>();

        var courseWriteModel = new CourseWriteModel {Name = newCourse.Name, Topic = newCourse.Topic!.Name};

        var response = await Client.PostAsJsonAsync("/Course/Create", courseWriteModel);
        response.EnsureSuccessStatusCode();

        using var repository = GetSchoolExamRepository();
        var courses = repository.List<Course>().ToList();
        courses.Should().HaveCount(2);
        var course = courses.Single(x => !x.Id.Equals(_course.Id));
        course.Name.Should().BeEquivalentTo(newCourse.Name);
        course.Topic!.Name.Should().BeEquivalentTo(newCourse.Topic!.Name);
        course.SchoolId.Should().Be(_school.Id);

        var teacherCourses = repository.List(new CourseTeacherByTeacherSpecification(_teacher.Id)).ToList();
        teacherCourses.Should().HaveCount(2);
    }
    
    [Test]
    public async Task CourseController_Update_CourseTeacher_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()));

        var updatedCourse = TestEntityFactory.Create<Course>();

        var courseWriteModel = new CourseWriteModel {Name = updatedCourse.Name, Topic = updatedCourse.Topic!.Name};

        var response = await Client.PutAsJsonAsync($"/Course/{_course.Id}/Update", courseWriteModel);
        response.EnsureSuccessStatusCode();

        using var repository = GetSchoolExamRepository();
        var courses = repository.List<Course>().ToList();
        courses.Should().HaveCount(1);
        var course = repository.Find<Course>(_course.Id)!;
        course.Name.Should().BeEquivalentTo(updatedCourse.Name);
        course.Topic!.Name.Should().BeEquivalentTo(updatedCourse.Topic!.Name);

        var teacherCourses = repository.List(new CourseTeacherByTeacherSpecification(_teacher.Id)).ToList();
        teacherCourses.Should().HaveCount(1);
    }
    
    [Test]
    public async Task CourseController_Update_CourseDoesNotExist_ThrowsException()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()));

        var updatedCourse = TestEntityFactory.Create<Course>();

        var courseWriteModel = new CourseWriteModel {Name = updatedCourse.Name, Topic = updatedCourse.Topic!.Name};

        var response = await Client.PutAsJsonAsync($"/Course/{Guid.NewGuid()}/Update", courseWriteModel);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Course does not exist.");
    }
    
    [Test]
    public async Task CourseController_Delete_CourseTeacher_ThrowsException()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()));

        var response = await Client.DeleteAsync($"/Course/{_course.Id}/Delete");
        response.EnsureSuccessStatusCode();

        using var repository = GetSchoolExamRepository();
        var courses = repository.List<Course>().ToList();
        courses.Should().HaveCount(0);
    }
    
    [Test]
    public async Task CourseController_Delete_CourseDoesNotExist_ThrowsException()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()));

        var response = await Client.DeleteAsync($"/Course/{Guid.NewGuid()}/Delete");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Course does not exist.");
    }
    
    [Test]
    public async Task CourseController_GetByTeacher_Teacher_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()));

        var response = await this.Client.GetAsync($"/Course/ByTeacher");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        var courses = JsonConvert.DeserializeObject<IEnumerable<CourseReadModelTeacher>>(result).ToList();
        courses.Should().HaveCount(1);

        var expectedCourse = new CourseReadModelTeacher
        {
            Id = _course.Id.ToString(), Name = _course.Name,
            Topic = _course.Topic?.Name, Students = new[]
            {
                new CourseStudentReadModel
                {
                    Id = _student.Id, FirstName = _student.FirstName, LastName = _student.LastName,
                    EmailAddress = _student.EmailAddress, DateOfBirth = _student.DateOfBirth
                }
            }
        };
        courses.Should().ContainEquivalentOf(expectedCourse);
    }
}