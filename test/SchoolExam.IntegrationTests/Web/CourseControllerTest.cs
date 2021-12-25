using System;
using System.Net;
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
using SchoolExam.IntegrationTests.Util;
using SchoolExam.Web.Course;

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
        _school = TestEntityFactory.Create<School, Guid>();
        _course = TestEntityFactory.Create<Course, Guid>();
        _course.SchoolId = _school.Id;
        _teacher = TestEntityFactory.Create<Teacher, Guid>();
        _teacher.SchoolId = _school.Id;
        var courseTeacher = new CourseTeacher(_course.Id, _teacher.Id);
        _student = TestEntityFactory.Create<Student, Guid>();
        _student.SchoolId = _school.Id;
        var courseStudent = new CourseStudent(_course.Id, _student.Id);

        using var context = GetSchoolExamDataContext();
        context.Add(_school);
        context.Add(_course);
        context.Add(_teacher);
        context.Add(courseTeacher);
        context.Add(_student);
        context.Add(courseStudent);
        await context.SaveChangesAsync();
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

        var expectedCourseDto = new CourseReadModelTeacher
        {
            Id = _course.Id.ToString(), Description = _course.Description, Name = _course.Name,
            Subject = _course.Subject?.Name, Year = _course.Year, StudentCount = 1
        };
        courseResult.Should().BeEquivalentTo(expectedCourseDto);
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
            Id = _course.Id.ToString(), Description = _course.Description, Name = _course.Name,
            Subject = _course.Subject?.Name, Year = _course.Year
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
}