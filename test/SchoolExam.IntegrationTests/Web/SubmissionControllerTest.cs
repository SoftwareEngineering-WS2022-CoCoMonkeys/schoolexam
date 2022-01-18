using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
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
using SchoolExam.Web.Models.Submission;

namespace SchoolExam.IntegrationTests.Web;

public class SubmissionControllerTest : ApiIntegrationTestBase
{
    private School _school = null!;
    private Course _course = null!;
    private Teacher _teacher = null!;
    private Student _student = null!, _otherStudent = null!;
    private Exam _exam = null!;
    private ExamTask _examTask = null!, _otherExamTask = null!;
    private User _user = null!;
    private Booklet _booklet1 = null!, _booklet2 = null!;
    private Submission _submission1 = null!, _submission2 = null!;
    private SubmissionPage _submission1Page = null!, _submission2Page = null!;

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
        _otherStudent = TestEntityFactory.Create<Student>();
        _otherStudent.SchoolId = _school.Id;
        _exam = TestEntityFactory.Create<Exam>();
        _exam.CreatorId = _teacher.Id;
        _exam.State = ExamState.InCorrection;
        _exam.TaskPdfFile!.ExamId = _exam.Id;
        _exam.GradingTable!.ExamId = _exam.Id;
        _examTask = TestEntityFactory.Create<ExamTask>();
        _examTask.ExamId = _exam.Id;
        _examTask.MaxPoints = 10;
        _examTask.Start = new ExamPosition(1, 200);
        _examTask.End = new ExamPosition(1, 100);
        _otherExamTask = TestEntityFactory.Create<ExamTask>();
        _otherExamTask.ExamId = _exam.Id;
        _otherExamTask.MaxPoints = 10;
        _otherExamTask.Start = new ExamPosition(2, 200);
        _otherExamTask.End = new ExamPosition(2, 100);
        var examCourse = new ExamCourse(_exam.Id, _course.Id);
        var examStudent = new ExamStudent(_exam.Id, _otherStudent.Id);
        _user = TestEntityFactory.Create<User>();
        _user.PersonId = _teacher.Id;
        
        _booklet1 = TestEntityFactory.Create<Booklet>();
        _booklet1.ExamId = _exam.Id;
        var booklet1Page = TestEntityFactory.Create<BookletPage>();
        booklet1Page.BookletId = _booklet1.Id;
        _submission1 = TestEntityFactory.Create<Submission>();
        _submission1.BookletId = _booklet1.Id;
        _submission1.StudentId = _student.Id;
        _submission1Page = TestEntityFactory.Create<SubmissionPage>();
        _submission1Page.ExamId = _exam.Id;
        _submission1Page.SubmissionId = _submission1.Id;
        _submission1Page.BookletPageId = booklet1Page.Id;
        var answer1Task1 = TestEntityFactory.Create<Answer>();
        answer1Task1.TaskId = _examTask.Id;
        answer1Task1.AchievedPoints = null;
        answer1Task1.State = AnswerState.Pending;
        answer1Task1.SubmissionId = _submission1.Id;
        var answer1Task2 = TestEntityFactory.Create<Answer>();
        answer1Task2.TaskId = _otherExamTask.Id;
        answer1Task2.AchievedPoints = 6;
        answer1Task2.State = AnswerState.Corrected;
        answer1Task2.SubmissionId = _submission1.Id;

        _booklet2 = TestEntityFactory.Create<Booklet>();
        _booklet2.ExamId = _exam.Id;
        var booklet2Page = TestEntityFactory.Create<BookletPage>();
        booklet2Page.BookletId = _booklet2.Id;
        _submission2 = TestEntityFactory.Create<Submission>();
        _submission2.BookletId = _booklet2.Id;
        _submission2.StudentId = _otherStudent.Id;
        _submission2Page = TestEntityFactory.Create<SubmissionPage>();
        _submission2Page.ExamId = _exam.Id;
        _submission2Page.SubmissionId = _submission2.Id;
        _submission2Page.BookletPageId = booklet2Page.Id;
        var answer2Task1 = TestEntityFactory.Create<Answer>();
        answer2Task1.SubmissionId = _submission2.Id;
        answer2Task1.TaskId = _examTask.Id;
        answer2Task1.AchievedPoints = 4;
        answer2Task1.State = AnswerState.Corrected;
        var answer2Task2 = TestEntityFactory.Create<Answer>();
        answer2Task2.SubmissionId = _submission2.Id;
        answer2Task2.TaskId = _otherExamTask.Id;
        answer2Task2.AchievedPoints = 10;
        answer2Task2.State = AnswerState.Corrected;

        var otherExam = TestEntityFactory.Create<Exam>();
        otherExam.State = ExamState.Planned;
        otherExam.CreatorId = _teacher.Id;
        var otherBooklet = TestEntityFactory.Create<Booklet>();
        otherBooklet.ExamId = otherExam.Id;
        var otherSubmission = TestEntityFactory.Create<Submission>();
        otherSubmission.BookletId = otherBooklet.Id;

        using var repository = GetSchoolExamRepository();
        repository.Add(_school);
        repository.Add(_course);
        repository.Add(_teacher);
        repository.Add(courseTeacher);
        repository.Add(_student);
        repository.Add(_otherStudent);
        repository.Add(courseStudent);
        repository.Add(_exam);
        repository.Add(_exam.TaskPdfFile!);
        repository.Add(_exam.GradingTable!);
        repository.Add(_examTask);
        repository.Add(_otherExamTask);
        repository.Add(examCourse);
        repository.Add(examStudent);
        repository.Add(_user);
        
        repository.Add(_booklet1);
        repository.Add(booklet1Page);
        repository.Add(_submission1);
        repository.Add(_submission1Page);
        repository.Add(answer1Task1);
        repository.Add(answer1Task2);
        
        repository.Add(_booklet2);
        repository.Add(booklet2Page);
        repository.Add(_submission2);
        repository.Add(_submission2Page);
        repository.Add(answer2Task1);
        repository.Add(answer2Task2);

        repository.Add(otherExam);
        repository.Add(otherBooklet);
        repository.Add(otherSubmission);
        await repository.SaveChangesAsync();
    }
    
    [Test]
    public async Task SubmissionController_Upload_AutomaticMatchingFails_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var uploadSubmissionsModel = new UploadSubmissionsModel
            {Pdf = Convert.ToBase64String(_booklet1.PdfFile.Content)};

        var response = await Client.PostAsJsonAsync($"/Submission/Upload/{_exam.Id}", uploadSubmissionsModel);
        response.EnsureSuccessStatusCode();

        using var repository = GetSchoolExamRepository();
        var submissionPages = repository.List(new SubmissionPageByExamSpecification(_exam.Id)).ToList();
        submissionPages.Should().HaveCount(4);
        var matchedSubmissionPageIds = submissionPages.Where(x => x.SubmissionId.HasValue).Select(x => x.Id).ToList();
        matchedSubmissionPageIds.Should().HaveCount(2);
        matchedSubmissionPageIds.Should().ContainEquivalentOf(_submission1Page.Id);
        matchedSubmissionPageIds.Should().ContainEquivalentOf(_submission2Page.Id);

        var bookletPages = repository.List(new BookletWithPagesByExamSpecification(_exam.Id)).SelectMany(x => x.Pages)
            .ToList();
        bookletPages.Should().HaveCount(2);
        bookletPages.Where(x => x.SubmissionPage != null).Should().HaveCount(2);

        var exam = repository.Find<Exam>(_exam.Id);
        exam?.State.Should().Be(ExamState.InCorrection);
    }

    [Test]
    public async Task SubmissionController_Upload_ExamNotBuiltPreviously_ThrowsException()
    {
        using (var repository = GetSchoolExamRepository())
        {
            var exam = repository.Find<Exam>(_exam.Id)!;
            exam.State = ExamState.Planned;
            repository.Update(exam);
            await repository.SaveChangesAsync();
        }

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var submissionPdf = TestEntityFactory.Create<SubmissionPagePdfFile>();

        var uploadSubmissionsModel = new UploadSubmissionsModel {Pdf = Convert.ToBase64String(submissionPdf.Content)};

        var response = await Client.PostAsJsonAsync($"/Submission/Upload/{_exam.Id}", uploadSubmissionsModel);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Exam is not ready to match submissions.");
    }
    
    [Test]
    public async Task SubmissionController_GetSubmissions_ExamCreator_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var response = await Client.GetAsync($"/Submission/ByExam/{_exam.Id}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<IEnumerable<SubmissionReadModel>>(content).ToList();

        var expectedSubmission1 = new SubmissionReadModel
        {
            Id = _submission1.Id, Status = CorrectionState.InProgress.ToString(), IsComplete = true,
            IsMatchedToStudent = true, AchievedPoints = 6,
            Student = new ExamStudentReadModel
                {Id = _student.Id, DisplayName = $"{_student.FirstName} {_student.LastName}"}
        };
        var expectedSubmission2 = new SubmissionReadModel
        {
            Id = _submission2.Id, Status = CorrectionState.Corrected.ToString(), IsComplete = true,
            IsMatchedToStudent = true, AchievedPoints = 14,
            Student = new ExamStudentReadModel
                {Id = _otherStudent.Id, DisplayName = $"{_otherStudent.FirstName} {_otherStudent.LastName}"}
        };

        result.Should().HaveCount(2);
        result.Should().ContainEquivalentOf(expectedSubmission1,
            x => x.Excluding(s => s.UpdatedAt));
        result.Should().ContainEquivalentOf(expectedSubmission2,
            x => x.Excluding(s => s.UpdatedAt));
    }
}