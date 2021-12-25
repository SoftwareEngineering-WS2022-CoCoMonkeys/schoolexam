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
    [Test]
    public async Task CourseController_GetByIdTeacherView_CourseTeacher_Success()
    {
        var school = TestEntityFactory.Create<School, Guid>();
        var course = TestEntityFactory.Create<Course, Guid>();
        course.SchoolId = school.Id;
        var teacher = TestEntityFactory.Create<Course, Guid>();
        teacher.SchoolId = school.Id;
        var courseTeacher = new CourseTeacher(course.Id, teacher.Id);
        var student = TestEntityFactory.Create<Student, Guid>();
        student.SchoolId = school.Id;
        var courseStudent = new CourseStudent(course.Id, student.Id);
        
        using (var context = GetSchoolExamDataContext())
        {
            context.Add(school);
            context.Add(course);
            context.Add(teacher);
            context.Add(courseTeacher);
            context.Add(student);
            context.Add(courseStudent);
            await context.SaveChangesAsync();
        }

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, teacher.Id.ToString()));
        
        var response = await this.Client.GetAsync($"/Course/{course.Id}/TeacherView");
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadAsStringAsync();
        var courseResult = JsonConvert.DeserializeObject<CourseReadModelTeacher?>(result);

        courseResult.Should().NotBeNull();

        var expectedCourseDto = new CourseReadModelTeacher
        {
            Id = course.Id.ToString(), Description = course.Description, Name = course.Name,
            Subject = course.Subject?.Name, Year = course.Year, StudentCount = 1
        };
        courseResult.Should().BeEquivalentTo(expectedCourseDto);
    }

    [Test]
    public async Task CourseController_GetByIdTeacherView_NoCourseTeacher_Unauthorized()
    {
        var school = TestEntityFactory.Create<School, Guid>();
        var course = TestEntityFactory.Create<Course, Guid>();
        course.SchoolId = school.Id;
        var teacher = TestEntityFactory.Create<Course, Guid>();
        teacher.SchoolId = school.Id;
        var courseTeacher = new CourseTeacher(course.Id, teacher.Id);

        using (var context = GetSchoolExamDataContext())
        {
            context.Add(school);
            context.Add(course);
            context.Add(teacher);
            context.Add(courseTeacher);
            await context.SaveChangesAsync();
        }
        
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, Guid.NewGuid().ToString()));
        
        var response = await this.Client.GetAsync($"/Course/{course.Id}/TeacherView");
        
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Test]
    public async Task CourseController_GetByIdStudentView_CourseStudent_Success()
    {
        var school = TestEntityFactory.Create<School, Guid>();
        var course = TestEntityFactory.Create<Course, Guid>();
        course.SchoolId = school.Id;
        var teacher = TestEntityFactory.Create<Course, Guid>();
        teacher.SchoolId = school.Id;
        var courseTeacher = new CourseTeacher(course.Id, teacher.Id);
        var student = TestEntityFactory.Create<Student, Guid>();
        student.SchoolId = school.Id;
        var courseStudent = new CourseStudent(course.Id, student.Id);
        
        using (var context = GetSchoolExamDataContext())
        {
            context.Add(school);
            context.Add(course);
            context.Add(teacher);
            context.Add(courseTeacher);
            context.Add(student);
            context.Add(courseStudent);
            await context.SaveChangesAsync();
        }

        SetClaims(new Claim(ClaimTypes.Role, Role.Student),
            new Claim(CustomClaimTypes.PersonId, student.Id.ToString()));
        
        var response = await this.Client.GetAsync($"/Course/{course.Id}/StudentView");
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadAsStringAsync();
        var courseResult = JsonConvert.DeserializeObject<CourseReadModelStudent?>(result);

        courseResult.Should().NotBeNull();

        var expectedCourseDto = new CourseReadModelStudent
        {
            Id = course.Id.ToString(), Description = course.Description, Name = course.Name,
            Subject = course.Subject?.Name, Year = course.Year
        };
        courseResult.Should().BeEquivalentTo(expectedCourseDto);
    }
    
    [Test]
    public async Task CourseController_GetByIdStudentView_NoCourseStudent_Unauthorized()
    {
        var school = TestEntityFactory.Create<School, Guid>();
        var course = TestEntityFactory.Create<Course, Guid>();
        course.SchoolId = school.Id;
        var teacher = TestEntityFactory.Create<Course, Guid>();
        teacher.SchoolId = school.Id;
        var courseTeacher = new CourseTeacher(course.Id, teacher.Id);
        var student = TestEntityFactory.Create<Student, Guid>();
        student.SchoolId = school.Id;
        var courseStudent = new CourseStudent(course.Id, student.Id);

        using (var context = GetSchoolExamDataContext())
        {
            context.Add(school);
            context.Add(course);
            context.Add(teacher);
            context.Add(courseTeacher);
            context.Add(student);
            context.Add(courseStudent);
            await context.SaveChangesAsync();
        }
        
        SetClaims(new Claim(ClaimTypes.Role, Role.Student),
            new Claim(CustomClaimTypes.PersonId, Guid.NewGuid().ToString()));
        
        var response = await this.Client.GetAsync($"/Course/{course.Id}/StudentView");
        
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}