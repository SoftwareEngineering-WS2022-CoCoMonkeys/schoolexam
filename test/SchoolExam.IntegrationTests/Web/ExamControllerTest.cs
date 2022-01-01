using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
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
using SchoolExam.IntegrationTests.Util;
using SchoolExam.IntegrationTests.Util.Extensions;
using SchoolExam.Web.Models.Exam;

namespace SchoolExam.IntegrationTests.Web;

public class ExamControllerTest : ApiIntegrationTestBase
{
    private School _school;
    private Course _course;
    private Teacher _teacher;
    private Exam _exam;
    private User _user;
    private TaskPdfFile _taskPdfFile;
    private ExamBooklet _booklet;
    private BookletPdfFile _bookletPdfFile;
    private ExamBookletPage _unmatchedBookletPage, _matchedBookletPage, _otherBookletPage;
    private Submission _submission;
    private SubmissionPage _unmatchedSubmissionPage, _matchedSubmissionPage, _otherSubmissionPage;

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
        _exam.CourseId = _course.Id;
        _taskPdfFile = TestEntityFactory.Create<TaskPdfFile, Guid>();
        _taskPdfFile.ExamId = _exam.Id;
        _user = TestEntityFactory.Create<User, Guid>();
        _user.PersonId = _teacher.Id;
        _booklet = TestEntityFactory.Create<ExamBooklet, Guid>();
        _booklet.ExamId = _exam.Id;
        _matchedBookletPage = TestEntityFactory.Create<ExamBookletPage, Guid>();
        _matchedBookletPage.BookletId = _booklet.Id;
        _unmatchedBookletPage = TestEntityFactory.Create<ExamBookletPage, Guid>();
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
        
        var otherExam = TestEntityFactory.Create<Exam, Guid>();
        otherExam.CreatorId = _teacher.Id;
        otherExam.CourseId = _course.Id;
        var otherTaskPdfFile = TestEntityFactory.Create<TaskPdfFile, Guid>();
        otherTaskPdfFile.ExamId = otherExam.Id;
        var otherBooklet = TestEntityFactory.Create<ExamBooklet, Guid>();
        otherBooklet.ExamId = otherExam.Id;
        _otherBookletPage = TestEntityFactory.Create<ExamBookletPage, Guid>();
        _otherBookletPage.BookletId = otherBooklet.Id;
        _otherSubmissionPage = TestEntityFactory.Create<SubmissionPage, Guid>();
        _otherSubmissionPage.ExamId = otherExam.Id;
        _otherSubmissionPage.SubmissionId = null;
        _otherSubmissionPage.BookletPageId = null;

        using var context = GetSchoolExamDataContext();
        context.Add(_school);
        context.Add(_course);
        context.Add(_teacher);
        context.Add(courseTeacher);
        context.Add(_exam);
        context.Add(_taskPdfFile);
        context.Add(_user);
        context.Add(_booklet);
        context.Add(_matchedBookletPage);
        context.Add(_unmatchedBookletPage);
        context.Add(_submission);
        context.Add(_matchedSubmissionPage);
        context.Add(_unmatchedSubmissionPage);
        
        context.Add(otherExam);
        context.Add(otherBooklet);
        context.Add(_otherBookletPage);
        context.Add(_otherSubmissionPage);
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
        await ResetExam();
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
            pages.Select(x => x.SubmissionId.HasValue).Should().AllBeEquivalentTo(true);
        }
    }
    
    [Test]
    public async Task ExamController_BuildAndMatch_ConcatSameSubmissionPdfTwice_Success()
    {
        await ResetExam();
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
            submissionPdf = pdfService.Merge(pdfs?.Concat(pdfs).ToArray() ?? Array.Empty<byte[]>());
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
            pages.Select(x => x.SubmissionId.HasValue).Should().AllBeEquivalentTo(true);
        }
    }

    [Test]
    public async Task ExamController_Build_ExamCountNotPositive_ThrowsException()
    {
        await ResetExam();
        int count = 0;
        
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var buildExamModel = new BuildExamModel {Count = count};

        var response = await this.Client.PostAsJsonAsync($"/Exam/{_exam.Id}/Build", buildExamModel);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(nameof(ArgumentException));
        content.Should().Contain("At least one exam booklet must be built.");
    }
    
    [Test]
    public async Task ExamController_Build_ExamBuiltPreviously_ThrowsException()
    {
        int count = 2;
        
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var buildExamModel = new BuildExamModel {Count = count};

        var response = await this.Client.PostAsJsonAsync($"/Exam/{_exam.Id}/Build", buildExamModel);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(nameof(InvalidOperationException));
        content.Should().Contain("Exam has already been built.");
    }
    
    [Test]
    public async Task ExamController_Build_TaskPdfFileMissing_ThrowsException()
    {
        await ResetExam();
        using (var context = GetSchoolExamDataContext())
        {
            var exam = context.Exams.Single(x => x.Id.Equals(_exam.Id));
            context.Remove(exam.TaskPdfFile!);
            await context.SaveChangesAsync();
        }
        
        int count = 2;
        
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var buildExamModel = new BuildExamModel {Count = count};

        var response = await this.Client.PostAsJsonAsync($"/Exam/{_exam.Id}/Build", buildExamModel);
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

        var fileName = "test-submission.pdf";

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var response = await this.Client.PostAsync($"/Exam/{_exam.Id}/Match",
            new MultipartFormDataContent
                {{new ByteArrayContent(_booklet.PdfFile.Content), "submissionPdfFormFile", fileName}});
        response.EnsureSuccessStatusCode();

        using (var context = GetSchoolExamDataContext())
        {
            var submissionPages = context.SubmissionPages.Where(x => x.ExamId.Equals(_exam.Id)).ToList();
            submissionPages.Should().HaveCount(4);
            submissionPages.Count(x => x.SubmissionId.HasValue).Should().Be(1);
            submissionPages.Single(x => x.SubmissionId.HasValue).Id.Should().Be(_matchedSubmissionPage.Id);

            var bookletPages = context.ExamBooklets.Where(x => x.ExamId.Equals(_exam.Id)).SelectMany(x => x.Pages);
            bookletPages.Should().HaveCount(2);
            bookletPages.Count(x => x.SubmissionPage != null).Should().Be(1);
            bookletPages.Single(x => x.SubmissionPage != null).Id.Should().Be(_matchedBookletPage.Id);
        }
    }
    
    [Test]
    public async Task ExamController_Clean_ExamCreator_Success()
    {
        using (var context = GetSchoolExamDataContext())
        {
            foreach (var submission in context.Submissions)
            {
                context.Remove(submission);
            }

            foreach (var submissionPage in context.SubmissionPages)
            {
                context.Remove(submissionPage);
            }
            await context.SaveChangesAsync();
        }

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var response = await this.Client.PostAsync($"/Exam/{_exam.Id}/Clean", null);
        response.EnsureSuccessStatusCode();
        
        using (var context = GetSchoolExamDataContext())
        {
            var exam = context.Exams.SingleOrDefault(x => x.Id == _exam.Id);
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

        using var context = GetSchoolExamDataContext();
        var submissionPages = context.SubmissionPages.Where(x => x.ExamId.Equals(_exam.Id));
        submissionPages.Select(x => x.SubmissionId.HasValue).Should().AllBeEquivalentTo(true);
        var exam = context.Exams.SingleOrDefault(x => x.Id.Equals(_exam.Id));
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
        using (var context = GetSchoolExamDataContext())
        {
            _matchedSubmissionPage.BookletPageId = null;
            context.Update(_matchedSubmissionPage);
            context.Remove(_submission);
            await context.SaveChangesAsync();
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
    }

    private async Task ResetExam()
    {
        using var context = GetSchoolExamDataContext();
        foreach (var booklet in context.ExamBooklets)
        {
            context.Remove(booklet);
        }

        foreach (var submissionPage in context.SubmissionPages)
        {
            context.Remove(submissionPage);
        }
        
        foreach (var submission in context.Submissions)
        {
            context.Remove(submission);
        }

        await context.SaveChangesAsync();
    }
}