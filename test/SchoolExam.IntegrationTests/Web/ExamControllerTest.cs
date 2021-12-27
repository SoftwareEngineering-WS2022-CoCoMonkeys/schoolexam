using System;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.Entities.SchoolAggregate;
using SchoolExam.Domain.Entities.UserAggregate;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Infrastructure.Authentication;
using SchoolExam.IntegrationTests.Util;

namespace SchoolExam.IntegrationTests.Web;

public class ExamControllerTest : ApiIntegrationTestBase
{
    private School _school;
    private Course _course;
    private Teacher _teacher;
    private Exam _exam;
    private User _user;
    
    protected override async void SetUpData()
    {
        _school = TestEntityFactory.Create<School, Guid>();
        _course = TestEntityFactory.Create<Course, Guid>();
        _course.SchoolId = _school.Id;
        _teacher = TestEntityFactory.Create<Teacher, Guid>();
        _teacher.SchoolId = _school.Id;
        var courseTeacher = new CourseTeacher(_course.Id, _teacher.Id);
        _exam = TestEntityFactory.Create<Exam, Guid>();
        _exam.CreatorId = _teacher.Id;
        _user = TestEntityFactory.Create<User, Guid>();
        _user.PersonId = _teacher.Id;

        using var context = GetSchoolExamDataContext();
        context.Add(_school);
        context.Add(_course);
        context.Add(_teacher);
        context.Add(courseTeacher);
        context.Add(_exam);
        await context.SaveChangesAsync();
    }

    [Test]
    public async Task ExamController_UploadTaskPdf_ExamCreator_Success()
    {
        var fileName = "test-exam.pdf";
        var content = Encoding.UTF8.GetBytes("This is a test file.");

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var response = await this.Client.PostAsync($"/Exam/{_exam.Id}/UploadTaskPdf",
            new MultipartFormDataContent {{new ByteArrayContent(content), "taskPdfFormFile", fileName}});
        response.EnsureSuccessStatusCode();

        using var context = GetSchoolExamDataContext();
        var exam = context.Find<Exam, Guid>(_exam.Id, nameof(Exam.TaskPdfFile));
        var taskPdfFile = exam?.TaskPdfFile;

        var expectedTaskPdfFile =
            new TaskPdfFile(Guid.Empty, fileName, content.Length, DateTime.Now, _user.Id, content, _exam.Id);

        taskPdfFile.Should().NotBeNull();
        using (new AssertionScope())
        {
            taskPdfFile!.Id.Should().NotBeEmpty();
            taskPdfFile.Should().BeEquivalentTo(expectedTaskPdfFile,
                opts => opts.Excluding(x => x.Id).Excluding(x => x.UploadedAt));
        }
    }
}