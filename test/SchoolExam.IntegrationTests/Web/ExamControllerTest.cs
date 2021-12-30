using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using SchoolExam.Application.Pdf;
using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.Entities.SchoolAggregate;
using SchoolExam.Domain.Entities.UserAggregate;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Infrastructure.Authentication;
using SchoolExam.IntegrationTests.Util;
using SchoolExam.IntegrationTests.Util.Extensions;
using SchoolExam.Web.Exam;

namespace SchoolExam.IntegrationTests.Web;

public class ExamControllerTest : ApiIntegrationTestBase
{
    private School _school;
    private Course _course;
    private Teacher _teacher;
    private Exam _exam;
    private User _user;
    private TaskPdfFile _taskPdfFile;

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
        _taskPdfFile = TestEntityFactory.Create<TaskPdfFile, Guid>();
        _taskPdfFile.ExamId = _exam.Id;
        _user = TestEntityFactory.Create<User, Guid>();
        _user.PersonId = _teacher.Id;

        using var context = GetSchoolExamDataContext();
        context.Add(_school);
        context.Add(_course);
        context.Add(_teacher);
        context.Add(courseTeacher);
        context.Add(_exam);
        context.Add(_taskPdfFile);
        context.Add(_user);
        await context.SaveChangesAsync();
    }

    [Test]
    public async Task ExamController_UploadTaskPdf_ExamCreator_Success()
    {
        var fileName = "test-exam.pdf";
        var content = Encoding.UTF8.GetBytes("This is a test exam.");

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

    [Test]
    public async Task ExamController_BuildAndMatch_ExamCreator_Success()
    {
        int count = 2;
        
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var buildExamModel = new BuildExamModel {Count = count};

        var buildResponse = await this.Client.PostAsJsonAsync($"/Exam/{_exam.Id}/Build", buildExamModel);
        buildResponse.EnsureSuccessStatusCode();

        byte[] submissionPdf;
        using (var context = GetSchoolExamDataContext())
        {
            var exam = context.Exams.SingleOrDefault(x => x.Id == _exam.Id);
            var booklets = exam?.Booklets;
            booklets.Should().HaveCount(2);
            var pages = booklets?.SelectMany(x => x.Pages);
            pages.Should().HaveCount(4);
            
            var pdfService = GetRequiredService<IPdfService>();
            var pdfs = booklets?.Select(x => x.PdfFile.Content).ToArray();
            submissionPdf = pdfService.Merge(pdfs ?? Array.Empty<byte[]>());
        }

        var fileName = "test-submission.pdf";

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var response = await this.Client.PostAsync($"/Exam/{_exam.Id}/Match",
            new MultipartFormDataContent {{new ByteArrayContent(submissionPdf), "submissionPdfFormFile", fileName}});
        response.EnsureSuccessStatusCode();

        using (var context = GetSchoolExamDataContext())
        {
            var pages = context.SubmissionPages.Where(x => x.ExamId.Equals(_exam.Id)).ToList();
            pages.Should().HaveCount(4);
        }
    }
}