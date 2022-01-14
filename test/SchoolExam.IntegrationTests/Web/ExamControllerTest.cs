using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NUnit.Framework;
using SchoolExam.Application.Pdf;
using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.Entities.SchoolAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;
using SchoolExam.Domain.Entities.UserAggregate;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Infrastructure.Authentication;
using SchoolExam.Infrastructure.Extensions;
using SchoolExam.Infrastructure.Specifications;
using SchoolExam.IntegrationTests.Util;
using SchoolExam.IntegrationTests.Util.Extensions;
using SchoolExam.IntegrationTests.Util.Specifications;
using SchoolExam.Web.Models.Exam;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SchoolExam.IntegrationTests.Web;

public class ExamControllerTest : ApiIntegrationTestBase
{
    private School _school = null!;
    private Course _course = null!;
    private Teacher _teacher = null!;
    private Student _student = null!, _otherStudent = null!;
    private Exam _exam = null!, _otherExam = null!;
    private User _user = null!;
    private TaskPdfFile _taskPdfFile = null!;
    private Booklet _booklet = null!;
    private BookletPage _unmatchedBookletPage = null!, _matchedBookletPage = null!, _otherBookletPage = null!;
    private Submission _submission = null!;

    private SubmissionPage _unmatchedSubmissionPage = null!,
        _matchedSubmissionPage = null!,
        _otherSubmissionPage = null!;

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
        _otherStudent = TestEntityFactory.Create<Student, Guid>();
        _student.SchoolId = _school.Id;
        _exam = TestEntityFactory.Create<Exam, Guid>();
        _exam.CreatorId = _teacher.Id;
        _exam.State = ExamState.SubmissionReady;
        var examCourse = new ExamCourse(_exam.Id, _course.Id);
        var examStudent = new ExamStudent(_exam.Id, _otherStudent.Id);
        _taskPdfFile = TestEntityFactory.Create<TaskPdfFile, Guid>();
        _taskPdfFile.ExamId = _exam.Id;
        _user = TestEntityFactory.Create<User, Guid>();
        _user.PersonId = _teacher.Id;
        _booklet = TestEntityFactory.Create<Booklet, Guid>();
        _booklet.ExamId = _exam.Id;
        _matchedBookletPage = TestEntityFactory.Create<BookletPage, Guid>();
        _matchedBookletPage.BookletId = _booklet.Id;
        _unmatchedBookletPage = TestEntityFactory.Create<BookletPage, Guid>();
        _unmatchedBookletPage.BookletId = _booklet.Id;
        _submission = TestEntityFactory.Create<Submission, Guid>();
        _submission.BookletId = _booklet.Id;
        _matchedSubmissionPage = TestEntityFactory.Create<SubmissionPage, Guid>();
        _matchedSubmissionPage.ExamId = _exam.Id;
        _matchedSubmissionPage.SubmissionId = _submission.Id;
        _matchedSubmissionPage.BookletPageId = _matchedBookletPage.Id;
        _unmatchedSubmissionPage = TestEntityFactory.Create<SubmissionPage, Guid>();
        _unmatchedSubmissionPage.ExamId = _exam.Id;
        _unmatchedSubmissionPage.SubmissionId = null;
        _unmatchedSubmissionPage.BookletPageId = null;

        _otherExam = TestEntityFactory.Create<Exam, Guid>();
        _otherExam.CreatorId = _teacher.Id;
        var otherExamCourse = new ExamCourse(_otherExam.Id, _course.Id);
        var otherTaskPdfFile = TestEntityFactory.Create<TaskPdfFile, Guid>();
        otherTaskPdfFile.ExamId = _otherExam.Id;
        var otherBooklet = TestEntityFactory.Create<Booklet, Guid>();
        otherBooklet.ExamId = _otherExam.Id;
        _otherBookletPage = TestEntityFactory.Create<BookletPage, Guid>();
        _otherBookletPage.BookletId = otherBooklet.Id;
        _otherSubmissionPage = TestEntityFactory.Create<SubmissionPage, Guid>();
        _otherSubmissionPage.ExamId = _otherExam.Id;
        _otherSubmissionPage.SubmissionId = null;
        _otherSubmissionPage.BookletPageId = null;

        using var repository = GetSchoolExamRepository();
        repository.Add(_school);
        repository.Add(_course);
        repository.Add(_teacher);
        repository.Add(courseTeacher);
        repository.Add(_student);
        repository.Add(_otherStudent);
        repository.Add(courseStudent);
        repository.Add(_exam);
        repository.Add(examCourse);
        repository.Add(examStudent);
        repository.Add(_taskPdfFile);
        repository.Add(_user);
        repository.Add(_booklet);
        repository.Add(_matchedBookletPage);
        repository.Add(_unmatchedBookletPage);
        repository.Add(_submission);
        repository.Add(_matchedSubmissionPage);
        repository.Add(_unmatchedSubmissionPage);

        repository.Add(_otherExam);
        repository.Add(otherExamCourse);
        repository.Add(otherBooklet);
        repository.Add(_otherBookletPage);
        repository.Add(_otherSubmissionPage);
        await repository.SaveChangesAsync();
    }

    [Test]
    public async Task ExamController_GetByTeacher_Success()
    {
        // add additional exam created by other teacher
        using (var repository = GetSchoolExamRepository())
        {
            var otherCourse = TestEntityFactory.Create<Course, Guid>();
            otherCourse.SchoolId = _school.Id;
            var otherTeacher = TestEntityFactory.Create<Teacher, Guid>();
            otherTeacher.SchoolId = _school.Id;
            var courseTeacher = new CourseTeacher(otherCourse.Id, otherTeacher.Id);
            var otherExam = TestEntityFactory.Create<Exam, Guid>();
            var otherExamCourse = new ExamCourse(otherExam.Id, otherCourse.Id);
            otherExam.CreatorId = otherTeacher.Id;
            otherExam.State = ExamState.Planned;
            repository.Add(otherCourse);
            repository.Add(otherTeacher);
            repository.Add(courseTeacher);
            repository.Add(otherExam);
            repository.Add(otherExamCourse);
            await repository.SaveChangesAsync();
        }

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var response = await this.Client.GetAsync($"/Exam/ByTeacher/");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        // make sure that custom JSON converter is used for deserialization
        var options = GetRequiredService<IOptions<JsonOptions>>();
        var exams = JsonSerializer
            .Deserialize<IEnumerable<ExamReadModelTeacher>>(result, options.Value.JsonSerializerOptions)?.ToList();

        var expectedExam1 = new ExamReadModelTeacher
        {
            Id = _exam.Id, Title = _exam.Title, Date = _exam.Date, Status = _exam.State, Topic = _exam.Topic.Name,
            Quota = null, DueDate = _exam.DueDate, Tasks = new List<ExamTaskReadModel>(), Participants =
                new List<ExamParticipantReadModel>
                {
                    new ExamCourseReadModel
                    {
                        Id = _course.Id, DisplayName = _course.Name, Children = new List<ExamStudentReadModel>
                        {
                            new() {Id = _student.Id, DisplayName = $"{_student.FirstName} {_student.LastName}"}
                        }
                    },
                    new ExamStudentReadModel
                        {Id = _otherStudent.Id, DisplayName = $"{_otherStudent.FirstName} {_otherStudent.LastName}"}
                }
        };
        var expectedExam2 = new ExamReadModelTeacher
        {
            Id = _otherExam.Id, Title = _otherExam.Title, Date = _otherExam.Date, Status = _otherExam.State,
            Topic = _otherExam.Topic.Name, Quota = null, DueDate = _otherExam.DueDate,
            Tasks = new List<ExamTaskReadModel>(),
            Participants = new List<ExamParticipantReadModel>
            {
                new ExamCourseReadModel
                {
                    Id = _course.Id, DisplayName = _course.Name, Children = new List<ExamStudentReadModel>
                    {
                        new() {Id = _student.Id, DisplayName = $"{_student.FirstName} {_student.LastName}"}
                    }
                }
            }
        };

        exams.Should().HaveCount(2);
        exams.Should().ContainEquivalentOf(expectedExam1);
        exams.Should().ContainEquivalentOf(expectedExam2);
    }

    [Test]
    public async Task ExamController_Create_CourseTeacher_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var newExam = TestEntityFactory.Create<Exam, Guid>();

        var examWriteModel = new ExamWriteModel
            {Title = newExam.Title, Description = newExam.Description, Date = newExam.Date, Topic = newExam.Topic.Name};

        var response = await this.Client.PostAsJsonAsync($"/Exam/Create", examWriteModel);
        response.EnsureSuccessStatusCode();

        using var repository = GetSchoolExamRepository();
        var exams = repository.List(new ExamByTeacherSpecification(_teacher.Id)).ToList();
        exams.Should().HaveCount(3);

        exams.Should().ContainEquivalentOf(newExam,
            opts => opts.Including(x => x.Title).Including(x => x.Description).Including(x => x.Date));
    }

    [Test]
    public async Task ExamController_Update_CourseTeacher_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var updatedExam = TestEntityFactory.Create<Exam, Guid>();
        updatedExam.Id = _exam.Id;

        var examWriteModel = new ExamWriteModel
        {
            Title = updatedExam.Title, Description = updatedExam.Description, Date = updatedExam.Date,
            Topic = updatedExam.Topic.Name
        };

        var response = await this.Client.PutAsJsonAsync($"/Exam/{_exam.Id}/Update", examWriteModel);
        response.EnsureSuccessStatusCode();

        using var repository = GetSchoolExamRepository();
        var exams = repository.List(new ExamByCourseSpecification(_course.Id)).ToList();
        exams.Should().HaveCount(2);

        exams.Should().ContainEquivalentOf(updatedExam,
            opts => opts.Including(x => x.Id).Including(x => x.Title).Including(x => x.Description)
                .Including(x => x.Date));
    }

    [Test]
    public async Task ExamController_Delete_ExamCreator_Success()
    {
        await ResetExam();

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var response = await this.Client.DeleteAsync($"/Exam/{_exam.Id}/Delete");
        response.EnsureSuccessStatusCode();

        using var repository = GetSchoolExamRepository();
        var exams = repository.List(new ExamByCourseSpecification(_course.Id)).ToList();
        exams.Should().HaveCount(1);

        exams.Should().NotContainEquivalentOf(_exam,
            opts => opts.Including(x => x.Id).Including(x => x.Title).Including(x => x.Description)
                .Including(x => x.Date));
    }

    [Test]
    public async Task ExamController_Delete_ExamBuiltPreviously_ThrowsException()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var response = await this.Client.DeleteAsync($"/Exam/{_exam.Id}/Delete");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(nameof(InvalidOperationException));
        content.Should().Contain("An exam that already has been built must not be deleted.");

        using var repository = GetSchoolExamRepository();
        var exams = repository.List(new ExamByCourseSpecification(_course.Id)).ToList();
        exams.Should().HaveCount(2);
    }

    [Test]
    public async Task ExamController_UploadTaskPdf_ExamCreator_Success()
    {
        await ResetExam();

        var content = TestEntityFactory.Create<TaskPdfFile, Guid>().Content;

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var uploadTaskPdfModel = new UploadTaskPdfModel
            {TaskPdf = Convert.ToBase64String(content), Tasks = new ExamTaskWriteModel[] { }};

        var response = await this.Client.PostAsJsonAsync($"/Exam/{_exam.Id}/UploadTaskPdf", uploadTaskPdfModel);
        response.EnsureSuccessStatusCode();

        using var repository = GetSchoolExamRepository();
        var exam = repository.Find(new ExamWithTaskPdfFileByIdSpecification(_exam.Id));
        exam?.State.Should().Be(ExamState.BuildReady);
        var taskPdfFile = exam?.TaskPdfFile;

        var expectedTaskPdfFile =
            new TaskPdfFile(Guid.Empty, $"{_exam.Id}.pdf", content.Length, DateTime.Now, _user.Id, content, _exam.Id);

        taskPdfFile.Should().NotBeNull();
        using (new AssertionScope())
        {
            taskPdfFile!.Id.Should().NotBeEmpty();
            taskPdfFile.Should().BeEquivalentTo(expectedTaskPdfFile,
                opts => opts.Excluding(x => x.Id).Excluding(x => x.UploadedAt).Excluding(x => x.Size)
                    .Excluding(x => x.Content));
        }
    }

    [Test]
    public async Task ExamController_UploadTaskPdf_ExamBuiltPreviously_ThrowsException()
    {
        var pdfContent = Encoding.UTF8.GetBytes("This is a test exam.");

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var uploadTaskPdfModel = new UploadTaskPdfModel
            {TaskPdf = Convert.ToBase64String(pdfContent), Tasks = new ExamTaskWriteModel[] { }};

        var response = await this.Client.PostAsJsonAsync($"/Exam/{_exam.Id}/UploadTaskPdf", uploadTaskPdfModel);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(nameof(InvalidOperationException));
        content.Should().Contain("The task PDF file of an exam that already has been built cannot be changed.");
    }

    [Test]
    public async Task ExamController_BuildAndMatch_ExamCreator_Success()
    {
        await ResetExam();

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var buildResponse = await this.Client.PostAsync($"/Exam/{_exam.Id}/Build", null);
        buildResponse.EnsureSuccessStatusCode();
        var buildContent = await buildResponse.Content.ReadAsStringAsync();
        var buildResult = JsonConvert.DeserializeObject<BuildResultModel>(buildContent);

        buildResult.Count.Should().Be(2);

        byte[] submissionPdf;
        using (var repository = GetSchoolExamRepository())
        {
            var exam = repository.Find(new ExamWithBookletsWithPagesAndPdfFileByIdSpecification(_exam.Id));
            exam?.State.Should().Be(ExamState.SubmissionReady);
            var booklets = exam?.Booklets;
            booklets.Should().HaveCount(2);
            var pages = booklets?.SelectMany(x => x.Pages);
            pages.Should().HaveCount(4);

            var pdfService = GetRequiredService<IPdfService>();
            var pdfs = booklets?.Select(x => x.PdfFile.Content).ToArray();
            submissionPdf = pdfService.Merge(pdfs ?? Array.Empty<byte[]>());
        }

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var submitAndMatchModel = new SubmitAndMatchModel {Pdf = Convert.ToBase64String(submissionPdf)};

        var response = await this.Client.PostAsJsonAsync($"/Exam/{_exam.Id}/SubmitAndMatch", submitAndMatchModel);
        response.EnsureSuccessStatusCode();

        using (var repository = GetSchoolExamRepository())
        {
            var exam = repository.Find<Exam, Guid>(_exam.Id);
            exam?.State.Should().Be(ExamState.InCorrection);
            var pages = repository.List(new SubmissionPageByExamSpecification(_exam.Id)).ToList();
            pages.Should().HaveCount(4);
            pages.Select(x => x.SubmissionId.HasValue).Should().AllBeEquivalentTo(true);
            var submissionIds = pages.Select(x => x.SubmissionId!.Value).ToHashSet();
            var submissions = repository.List(new SubmissionWithPdfFileByIdsSpecification(submissionIds));
            submissions.Select(x => x.PdfFile != null).Should().AllBeEquivalentTo(true);
        }
    }

    [Test]
    public async Task ExamController_BuildAndMatch_ConcatSameSubmissionPdfTwice_Success()
    {
        await ResetExam();

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var buildResponse = await this.Client.PostAsync($"/Exam/{_exam.Id}/Build", null);
        buildResponse.EnsureSuccessStatusCode();
        var buildContent = await buildResponse.Content.ReadAsStringAsync();
        var buildResult = JsonConvert.DeserializeObject<BuildResultModel>(buildContent);

        buildResult.Count.Should().Be(2);

        byte[] submissionPdf;
        using (var repository = GetSchoolExamRepository())
        {
            var exam = repository.Find(new ExamWithBookletsWithPagesAndPdfFileByIdSpecification(_exam.Id))!;
            exam.State.Should().Be(ExamState.SubmissionReady);
            var booklets = exam?.Booklets;
            booklets.Should().HaveCount(2);
            var pages = booklets?.SelectMany(x => x.Pages);
            pages.Should().HaveCount(4);

            var pdfService = GetRequiredService<IPdfService>();
            var pdfs = booklets?.Select(x => x.PdfFile.Content).ToArray();
            submissionPdf = pdfService.Merge(pdfs?.Concat(pdfs).ToArray() ?? Array.Empty<byte[]>());
        }

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var submitAndMatchModel = new SubmitAndMatchModel {Pdf = Convert.ToBase64String(submissionPdf)};

        var response = await this.Client.PostAsJsonAsync($"/Exam/{_exam.Id}/SubmitAndMatch", submitAndMatchModel);

        response.EnsureSuccessStatusCode();

        using (var repository = GetSchoolExamRepository())
        {
            var exam = repository.Find<Exam, Guid>(_exam.Id);
            exam?.State.Should().Be(ExamState.InCorrection);
            var pages = repository.List(new SubmissionPageByExamSpecification(_exam.Id)).ToList();
            pages.Should().HaveCount(4);
            pages.Select(x => x.SubmissionId.HasValue).Should().AllBeEquivalentTo(true);
            var submissionIds = pages.Select(x => x.SubmissionId!.Value).ToHashSet();
            var submissions = repository.List(new SubmissionWithPdfFileByIdsSpecification(submissionIds));
            submissions.Select(x => x.PdfFile != null).Should().AllBeEquivalentTo(true);
        }
    }

    [Test]
    public async Task ExamController_Build_ExamCountNotPositive_ThrowsException()
    {
        await ResetExam();
        
        // remove all participants
        using (var repository = GetSchoolExamRepository())
        {
            var examCourses = repository.List<ExamCourse>();
            foreach (var examCourse in examCourses)
            {
                repository.Remove(examCourse);
            }

            var examStudents = repository.List<ExamStudent>();
            foreach (var examStudent in examStudents)
            {
                repository.Remove(examStudent);
            }

            await repository.SaveChangesAsync();
        }

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var response = await this.Client.PostAsync($"/Exam/{_exam.Id}/Build", null);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(nameof(ArgumentException));
        content.Should().Contain("At least one exam booklet must be built.");
    }

    [Test]
    public async Task ExamController_Build_ExamBuiltPreviously_ThrowsException()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var response = await this.Client.PostAsync($"/Exam/{_exam.Id}/Build", null);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(nameof(InvalidOperationException));
        content.Should().Contain("Exam has already been built.");
    }

    [Test]
    public async Task ExamController_Build_TaskPdfFileMissing_ThrowsException()
    {
        await ResetExam();
        using (var repository = GetSchoolExamRepository())
        {
            var exam = repository.Find(new ExamWithTaskPdfFileByIdSpecification(_exam.Id))!;
            repository.Remove(exam.TaskPdfFile!);
            await repository.SaveChangesAsync();
        }

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var response = await this.Client.PostAsync($"/Exam/{_exam.Id}/Build", null);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(nameof(InvalidOperationException));
        content.Should().Contain("Exam does not have a task PDF file.");
    }

    [Test]
    public async Task ExamController_Match_AutomaticMatchingFails_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var submitAndMatchModel = new SubmitAndMatchModel {Pdf = Convert.ToBase64String(_booklet.PdfFile.Content)};

        var response = await this.Client.PostAsJsonAsync($"/Exam/{_exam.Id}/SubmitAndMatch", submitAndMatchModel);
        response.EnsureSuccessStatusCode();

        using var repository = GetSchoolExamRepository();
        var submissionPages = repository.List(new SubmissionPageByExamSpecification(_exam.Id)).ToList();
        submissionPages.Should().HaveCount(4);
        submissionPages.Count(x => x.SubmissionId.HasValue).Should().Be(1);
        submissionPages.Single(x => x.SubmissionId.HasValue).Id.Should().Be(_matchedSubmissionPage.Id);

        var bookletPages = repository.List(new BookletWithPagesByExamSpecification(_exam.Id)).SelectMany(x => x.Pages)
            .ToList();
        bookletPages.Should().HaveCount(2);
        bookletPages.Count(x => x.SubmissionPage != null).Should().Be(1);
        bookletPages.Single(x => x.SubmissionPage != null).Id.Should().Be(_matchedBookletPage.Id);

        var exam = repository.Find<Exam, Guid>(_exam.Id);
        exam?.State.Should().Be(ExamState.SubmissionReady);
    }

    [Test]
    public async Task ExamController_Match_ExamNotBuiltPreviously_ThrowsException()
    {
        await ResetExam();

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var submissionPdf = TestEntityFactory.Create<SubmissionPagePdfFile, Guid>();

        var submitAndMatchModel = new SubmitAndMatchModel {Pdf = Convert.ToBase64String(submissionPdf.Content)};

        var response = await this.Client.PostAsJsonAsync($"/Exam/{_exam.Id}/SubmitAndMatch", submitAndMatchModel);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(nameof(InvalidOperationException));
        content.Should().Contain("Exam is not ready to match submissions.");
    }

    [Test]
    public async Task ExamController_Clean_ExamCreator_Success()
    {
        using (var repository = GetSchoolExamRepository())
        {
            foreach (var submission in repository.List<Submission>())
            {
                repository.Remove(submission);
            }

            foreach (var submissionPage in repository.List<SubmissionPage>())
            {
                repository.Remove(submissionPage);
            }

            await repository.SaveChangesAsync();
        }

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var response = await this.Client.PostAsync($"/Exam/{_exam.Id}/Clean", null);
        response.EnsureSuccessStatusCode();

        using (var repository = GetSchoolExamRepository())
        {
            var exam = repository.Find(new ExamWithBookletsByIdSpecification(_exam.Id));
            exam?.State.Should().Be(ExamState.BuildReady);
            var booklets = exam?.Booklets;
            booklets.Should().HaveCount(0);
        }
    }

    [Test]
    public async Task ExamController_Clean_ExamNotBuiltPreviously_ThrowsException()
    {
        await ResetExam();

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var response = await this.Client.PostAsync($"/Exam/{_exam.Id}/Clean", null);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(nameof(InvalidOperationException));
        content.Should().Contain("The exam has not been built yet.");
    }

    [Test]
    public async Task ExamController_Clean_ExamWithSubmissionPages_ThrowsException()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var response = await this.Client.PostAsync($"/Exam/{_exam.Id}/Clean", null);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(nameof(InvalidOperationException));
        content.Should().Contain("An exam with existing submission pages must not be cleaned.");
    }

    [Test]
    public async Task ExamController_GetUnmatchedPages_ExamCreator_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var response = await this.Client.GetAsync($"/Exam/{_exam.Id}/UnmatchedPages");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        var pagesResult = JsonConvert.DeserializeObject<UnmatchedPagesReadModel>(result);

        pagesResult.UnmatchedBookletPages.Should().HaveCount(1);
        pagesResult.UnmatchedBookletPages.Should().ContainSingle(x => x.Id.Equals(_unmatchedBookletPage.Id));
        pagesResult.UnmatchedSubmissionPages.Should().HaveCount(1);
        pagesResult.UnmatchedSubmissionPages.Should().ContainSingle(x => x.Id.Equals(_unmatchedSubmissionPage.Id));
    }

    [Test]
    public async Task ExamController_GetUnmatchedPages_ExamDoesNotExist_ThrowsException()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var examId = Guid.NewGuid();

        var response = await this.Client.GetAsync($"/Exam/{examId}/UnmatchedPages");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(nameof(ArgumentException));
        content.Should().Contain("Exam does not exist.");
    }

    [Test]
    public async Task ExamController_MatchManually_ExamCreator_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var manualMatchesModel = new ManualMatchesModel
        {
            Matches = new[]
            {
                new ManualMatchModel
                    {BookletPageId = _unmatchedBookletPage.Id, SubmissionPageId = _unmatchedSubmissionPage.Id}
            }
        };

        var response = await this.Client.PostAsJsonAsync($"/Exam/{_exam.Id}/MatchPages", manualMatchesModel);
        response.EnsureSuccessStatusCode();

        using var repository = GetSchoolExamRepository();
        var submissionPages = repository.List(new SubmissionPageByExamSpecification(_exam.Id));
        submissionPages.Select(x => x.SubmissionId.HasValue).Should().AllBeEquivalentTo(true);
        var exam = repository.Find(new ExamWithBookletsWithPagesByIdSpecification(_exam.Id));
        exam?.State.Should().Be(ExamState.InCorrection);
        var bookletPages = exam?.Booklets.SelectMany(x => x.Pages);
        bookletPages?.Select(x => x.SubmissionPage != null).Should().AllBeEquivalentTo(true);
    }

    [Test]
    public async Task ExamController_MatchManually_BookletPageAlreadyMatched_ThrowsException()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var manualMatchesModel = new ManualMatchesModel
        {
            Matches = new[]
            {
                new ManualMatchModel
                    {BookletPageId = _matchedBookletPage.Id, SubmissionPageId = _unmatchedSubmissionPage.Id}
            }
        };

        var response = await this.Client.PostAsJsonAsync($"/Exam/{_exam.Id}/MatchPages", manualMatchesModel);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(nameof(InvalidOperationException));
        content.Should().Contain("The booklet page and/or the submission page have already been matched.");
    }

    [Test]
    public async Task ExamController_MatchManually_SubmissionPageAlreadyMatched_ThrowsException()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var manualMatchesModel = new ManualMatchesModel
        {
            Matches = new[]
            {
                new ManualMatchModel
                    {BookletPageId = _unmatchedBookletPage.Id, SubmissionPageId = _matchedSubmissionPage.Id}
            }
        };

        var response = await this.Client.PostAsJsonAsync($"/Exam/{_exam.Id}/MatchPages", manualMatchesModel);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(nameof(InvalidOperationException));
        content.Should().Contain("The booklet page and/or the submission page have already been matched.");
    }

    [Test]
    public async Task ExamController_MatchManually_BookletPageDoesNotExist_ThrowsException()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var bookletPageId = Guid.NewGuid();
        var manualMatchesModel = new ManualMatchesModel
        {
            Matches = new[]
            {
                new ManualMatchModel
                    {BookletPageId = bookletPageId, SubmissionPageId = _unmatchedSubmissionPage.Id}
            }
        };

        var response = await this.Client.PostAsJsonAsync($"/Exam/{_exam.Id}/MatchPages", manualMatchesModel);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(nameof(ArgumentException));
        content.Should().Contain("Booklet page does not exist.");
    }

    [Test]
    public async Task ExamController_MatchManually_SubmissionPageDoesNotExist_ThrowsException()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var submissionPageId = Guid.NewGuid();
        var manualMatchesModel = new ManualMatchesModel
        {
            Matches = new[]
            {
                new ManualMatchModel
                    {BookletPageId = _unmatchedBookletPage.Id, SubmissionPageId = submissionPageId}
            }
        };

        var response = await this.Client.PostAsJsonAsync($"/Exam/{_exam.Id}/MatchPages", manualMatchesModel);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(nameof(ArgumentException));
        content.Should().Contain("Submission page does not exist.");
    }

    [Test]
    public async Task ExamController_MatchManually_BookletPageFromOtherExam_ThrowsException()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var manualMatchesModel = new ManualMatchesModel
        {
            Matches = new[]
            {
                new ManualMatchModel
                    {BookletPageId = _otherBookletPage.Id, SubmissionPageId = _unmatchedSubmissionPage.Id}
            }
        };

        var response = await this.Client.PostAsJsonAsync($"/Exam/{_exam.Id}/MatchPages", manualMatchesModel);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(nameof(InvalidOperationException));
        content.Should().Contain("Booklet page is not part of the exam.");
    }

    [Test]
    public async Task ExamController_MatchManually_SubmissionPageFromOtherExam_ThrowsException()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var manualMatchesModel = new ManualMatchesModel
        {
            Matches = new[]
            {
                new ManualMatchModel
                    {BookletPageId = _unmatchedBookletPage.Id, SubmissionPageId = _otherSubmissionPage.Id}
            }
        };

        var response = await this.Client.PostAsJsonAsync($"/Exam/{_exam.Id}/MatchPages", manualMatchesModel);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(nameof(InvalidOperationException));
        content.Should().Contain("Submission page is not part of the exam.");
    }

    [Test]
    public async Task ExamController_MatchManually_NoSubmissionCreatedPreviously_Success()
    {
        using (var repository = GetSchoolExamRepository())
        {
            _matchedSubmissionPage.BookletPageId = null;
            _matchedSubmissionPage.SubmissionId = null;
            _matchedBookletPage.SubmissionPage = null;
            repository.Update(_matchedSubmissionPage);
            repository.Remove(_submission);
            await repository.SaveChangesAsync();
        }

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var manualMatchesModel = new ManualMatchesModel
        {
            Matches = new[]
            {
                new ManualMatchModel
                    {BookletPageId = _unmatchedBookletPage.Id, SubmissionPageId = _unmatchedSubmissionPage.Id}
            }
        };

        var response = await this.Client.PostAsJsonAsync($"/Exam/{_exam.Id}/MatchPages", manualMatchesModel);
        response.EnsureSuccessStatusCode();

        using (var repository = GetSchoolExamRepository())
        {
            var submissionPages = repository.List(new SubmissionPageByExamSpecification(_exam.Id));
            submissionPages.Count(x => x.SubmissionId.HasValue).Should().Be(1);
            var exam = repository.Find(new ExamWithBookletsWithPagesByIdSpecification(_exam.Id));
            exam?.State.Should().Be(ExamState.SubmissionReady);
            var bookletPages = exam?.Booklets.SelectMany(x => x.Pages);
            bookletPages?.Count(x => x.SubmissionPage != null).Should().Be(1);
        }
    }

    private async Task ResetExam()
    {
        using var repository = GetSchoolExamRepository();
        foreach (var booklet in repository.List<Booklet>())
        {
            repository.Remove(booklet);
        }

        foreach (var submissionPage in repository.List<SubmissionPage>())
        {
            repository.Remove(submissionPage);
        }

        foreach (var submission in repository.List<Submission>())
        {
            repository.Remove(submission);
        }

        var exam = repository.Find<Exam, Guid>(_exam.Id)!;
        exam.State = ExamState.BuildReady;
        repository.Update(exam);

        await repository.SaveChangesAsync();
    }
}