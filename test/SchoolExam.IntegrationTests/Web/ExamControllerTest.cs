using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Equivalency;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NUnit.Framework;
using SchoolExam.Application.Pdf;
using SchoolExam.Application.QrCode;
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
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SchoolExam.IntegrationTests.Web;

public class ExamControllerTest : ApiIntegrationTestBase
{
    private School _school = null!;
    private Course _course = null!;
    private Teacher _teacher = null!;
    private Student _student = null!, _otherStudent = null!;
    private Exam _exam = null!, _otherExam = null!;
    private ExamTask _examTask = null!, _otherExamTask = null!;
    private User _user = null!;
    private Booklet _booklet = null!;
    private BookletPage _unmatchedBookletPage = null!, _matchedBookletPage = null!, _otherBookletPage = null!;
    private Submission _submission = null!;

    private SubmissionPage _unmatchedSubmissionPage = null!,
        _matchedSubmissionPage = null!,
        _otherSubmissionPage = null!;

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
        _exam.State = ExamState.SubmissionReady;
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
        _booklet = TestEntityFactory.Create<Booklet>();
        _booklet.ExamId = _exam.Id;
        _matchedBookletPage = TestEntityFactory.Create<BookletPage>();
        _matchedBookletPage.BookletId = _booklet.Id;
        _unmatchedBookletPage = TestEntityFactory.Create<BookletPage>();
        _unmatchedBookletPage.BookletId = _booklet.Id;
        _submission = TestEntityFactory.Create<Submission>();
        _submission.BookletId = _booklet.Id;
        _matchedSubmissionPage = TestEntityFactory.Create<SubmissionPage>();
        _matchedSubmissionPage.ExamId = _exam.Id;
        _matchedSubmissionPage.SubmissionId = _submission.Id;
        _matchedSubmissionPage.BookletPageId = _matchedBookletPage.Id;
        _unmatchedSubmissionPage = TestEntityFactory.Create<SubmissionPage>();
        _unmatchedSubmissionPage.ExamId = _exam.Id;
        _unmatchedSubmissionPage.SubmissionId = null;
        _unmatchedSubmissionPage.BookletPageId = null;

        _otherExam = TestEntityFactory.Create<Exam>();
        _otherExam.CreatorId = _teacher.Id;
        _otherExam.TaskPdfFile!.ExamId = _otherExam.Id;
        _otherExam.GradingTable!.ExamId = _otherExam.Id;
        var otherExamCourse = new ExamCourse(_otherExam.Id, _course.Id);
        var otherBooklet = TestEntityFactory.Create<Booklet>();
        otherBooklet.ExamId = _otherExam.Id;
        _otherBookletPage = TestEntityFactory.Create<BookletPage>();
        _otherBookletPage.BookletId = otherBooklet.Id;
        _otherSubmissionPage = TestEntityFactory.Create<SubmissionPage>();
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
        repository.Add(_exam.TaskPdfFile!);
        repository.Add(_exam.GradingTable!);
        repository.Add(_examTask);
        repository.Add(_otherExamTask);
        repository.Add(examCourse);
        repository.Add(examStudent);
        repository.Add(_user);
        repository.Add(_booklet);
        repository.Add(_matchedBookletPage);
        repository.Add(_unmatchedBookletPage);
        repository.Add(_submission);
        repository.Add(_matchedSubmissionPage);
        repository.Add(_unmatchedSubmissionPage);

        repository.Add(_otherExam);
        repository.Add(_otherExam.TaskPdfFile!);
        repository.Add(_otherExam.GradingTable!);
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
            var otherCourse = TestEntityFactory.Create<Course>();
            otherCourse.SchoolId = _school.Id;
            var otherTeacher = TestEntityFactory.Create<Teacher>();
            otherTeacher.SchoolId = _school.Id;
            var courseTeacher = new CourseTeacher(otherCourse.Id, otherTeacher.Id);
            var otherExam = TestEntityFactory.Create<Exam>();
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

        var response = await Client.GetAsync($"/Exam/ByTeacher/");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        // make sure that custom JSON converter is used for deserialization
        var options = GetRequiredService<IOptions<JsonOptions>>();
        var exams = JsonSerializer
            .Deserialize<IEnumerable<ExamReadModelTeacher>>(result, options.Value.JsonSerializerOptions)?.ToList();

        var maxPoints1 = _exam.GradingTable!.Intervals.Max(x => x.End.Points);
        var expectedExam1 = new ExamReadModelTeacher
        {
            Id = _exam.Id, Title = _exam.Title, Date = _exam.Date, Status = _exam.State.ToString(),
            Topic = _exam.Topic.Name,
            Quota = null, DueDate = _exam.DueDate, Tasks = new List<ExamTaskReadModel>
            {
                new() {Id = _examTask.Id, Title = _examTask.Title, MaxPoints = _examTask.MaxPoints},
                new() {Id = _otherExamTask.Id, Title = _otherExamTask.Title, MaxPoints = _otherExamTask.MaxPoints}
            }, Participants =
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
                },
            GradingTable = new GradingTableReadModel
            {
                LowerBounds = _exam.GradingTable!.Intervals
                    .Select<GradingTableInterval, GradingTableLowerBoundModelBase>(x =>
                        x.Type == GradingTableLowerBoundType.Points
                            ? new GradingTableLowerBoundPointsModel {Grade = x.Grade, Points = x.Start.Points}
                            : new GradingTableLowerBoundPercentageModel
                                {Grade = x.Grade, Percentage = x.Start.Points / maxPoints1 * 100})
                    .ToList()
            }
        };

        var maxPoints2 = _otherExam.GradingTable!.Intervals.Max(x => x.End.Points);
        var expectedExam2 = new ExamReadModelTeacher
        {
            Id = _otherExam.Id, Title = _otherExam.Title, Date = _otherExam.Date, Status = _otherExam.State.ToString(),
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
            },
            GradingTable = new GradingTableReadModel
            {
                LowerBounds = _otherExam.GradingTable!.Intervals
                    .Select<GradingTableInterval, GradingTableLowerBoundModelBase>(x =>
                        x.Type == GradingTableLowerBoundType.Points
                            ? new GradingTableLowerBoundPointsModel {Grade = x.Grade, Points = x.Start.Points}
                            : new GradingTableLowerBoundPercentageModel
                                {Grade = x.Grade, Percentage = x.Start.Points * maxPoints2 / 100})
                    .ToList()
            }
        };

        exams.Should().HaveCount(2);
        exams.Should().ContainEquivalentOf(expectedExam1, x => x.Excluding(e => e.Quota));
        exams.Should().ContainEquivalentOf(expectedExam2, x => x.Excluding(e => e.Quota));
    }

    [Test]
    public async Task ExamController_Create_CourseTeacher_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var newExam = TestEntityFactory.Create<Exam>();

        var examWriteModel = new ExamWriteModel
            {Title = newExam.Title, Date = newExam.Date, Topic = newExam.Topic.Name};

        var response = await Client.PostAsJsonAsync($"/Exam/Create", examWriteModel);
        response.EnsureSuccessStatusCode();

        using var repository = GetSchoolExamRepository();
        var exams = repository.List(new ExamByTeacherSpecification(_teacher.Id)).ToList();
        exams.Should().HaveCount(3);

        exams.Should().ContainEquivalentOf(newExam,
            opts => opts.Including(x => x.Title).Including(x => x.Date));
    }

    [Test]
    public async Task ExamController_Update_CourseTeacher_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var updatedExam = TestEntityFactory.Create<Exam>();
        updatedExam.Id = _exam.Id;

        var examWriteModel = new ExamWriteModel
        {
            Title = updatedExam.Title, Date = updatedExam.Date,
            Topic = updatedExam.Topic.Name
        };

        var response = await Client.PutAsJsonAsync($"/Exam/{_exam.Id}/Update", examWriteModel);
        response.EnsureSuccessStatusCode();

        using var repository = GetSchoolExamRepository();
        var exams = repository.List(new ExamByCourseSpecification(_course.Id)).ToList();
        exams.Should().HaveCount(2);

        exams.Should().ContainEquivalentOf(updatedExam,
            opts => opts.Including(x => x.Id).Including(x => x.Title)
                .Including(x => x.Date));
    }

    [Test]
    public async Task ExamController_Delete_ExamCreator_Success()
    {
        await ResetExam();

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var response = await Client.DeleteAsync($"/Exam/{_exam.Id}/Delete");
        response.EnsureSuccessStatusCode();

        using var repository = GetSchoolExamRepository();
        var exams = repository.List(new ExamByCourseSpecification(_course.Id)).ToList();
        exams.Should().HaveCount(1);

        exams.Should().NotContainEquivalentOf(_exam,
            opts => opts.Including(x => x.Id).Including(x => x.Title)
                .Including(x => x.Date));
    }

    [Test]
    public async Task ExamController_Delete_ExamBuiltPreviously_ThrowsException()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var response = await Client.DeleteAsync($"/Exam/{_exam.Id}/Delete");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("An exam that already has been built must not be deleted.");

        using var repository = GetSchoolExamRepository();
        var exams = repository.List(new ExamByCourseSpecification(_course.Id)).ToList();
        exams.Should().HaveCount(2);
    }

    [Test]
    public async Task ExamController_SetParticipants_ExamCreator_Success()
    {
        await ResetExam();

        var course = TestEntityFactory.Create<Course>();
        var student = TestEntityFactory.Create<Student>();
        using (var repository = GetSchoolExamRepository())
        {
            repository.Add(course);
            repository.Add(student);
            await repository.SaveChangesAsync();
        }

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var setParticipantsModel = new SetParticipantsModel
        {
            Participants = new ExamParticipantWriteModel[]
            {
                new ExamCourseWriteModel {Id = course.Id},
                new ExamStudentWriteModel {Id = student.Id}
            }
        };

        var options = GetRequiredService<IOptions<JsonOptions>>();

        var response = await Client.PostAsJsonAsync($"/Exam/{_exam.Id}/SetParticipants", setParticipantsModel,
            options.Value.JsonSerializerOptions);
        response.EnsureSuccessStatusCode();

        var expectedExamCourse = new ExamCourse(_exam.Id, course.Id);
        var expectedExamStudent = new ExamStudent(_exam.Id, student.Id);

        using (var repository = GetSchoolExamRepository())
        {
            var exam = repository.Find(new ExamWithParticipantsById(_exam.Id))!;
            exam.Participants.Should().HaveCount(2);
            exam.Participants.Should().ContainEquivalentOf(expectedExamCourse,
                x => x.Including(e => e.ExamId).Including(e => e.ParticipantId));
            exam.Participants.Should().ContainEquivalentOf(expectedExamStudent,
                x => x.Including(e => e.ExamId).Including(e => e.ParticipantId));
        }
    }

    [Test]
    public async Task ExamController_SetParticipants_ExamBuiltPreviously_ThrowsException()
    {
        var course = TestEntityFactory.Create<Course>();
        var student = TestEntityFactory.Create<Student>();
        using (var repository = GetSchoolExamRepository())
        {
            repository.Add(course);
            repository.Add(student);
            await repository.SaveChangesAsync();
        }

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var setParticipantsModel = new SetParticipantsModel
        {
            Participants = new ExamParticipantWriteModel[]
            {
                new ExamCourseWriteModel {Id = course.Id},
                new ExamStudentWriteModel {Id = student.Id}
            }
        };

        var options = GetRequiredService<IOptions<JsonOptions>>();

        var response = await Client.PostAsJsonAsync($"/Exam/{_exam.Id}/SetParticipants", setParticipantsModel,
            options.Value.JsonSerializerOptions);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("The participants of an exam that already has been built cannot be changed.");

        var expectedExamCourse = new ExamCourse(_exam.Id, _course.Id);
        var expectedExamStudent = new ExamStudent(_exam.Id, _otherStudent.Id);

        using (var repository = GetSchoolExamRepository())
        {
            var exam = repository.Find(new ExamWithParticipantsById(_exam.Id))!;
            exam.Participants.Should().HaveCount(2);
            exam.Participants.Should().ContainEquivalentOf(expectedExamCourse,
                x => x.Including(e => e.ExamId).Including(e => e.ParticipantId));
            exam.Participants.Should().ContainEquivalentOf(expectedExamStudent,
                x => x.Including(e => e.ExamId).Including(e => e.ParticipantId));
        }
    }

    [Test]
    public async Task ExamController_SetParticipants_CourseDoesNotExist_ThrowsException()
    {
        await ResetExam();

        var student = TestEntityFactory.Create<Student>();
        using (var repository = GetSchoolExamRepository())
        {
            repository.Add(student);
            await repository.SaveChangesAsync();
        }

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var setParticipantsModel = new SetParticipantsModel
        {
            Participants = new ExamParticipantWriteModel[]
            {
                new ExamCourseWriteModel {Id = Guid.NewGuid()},
                new ExamStudentWriteModel {Id = student.Id}
            }
        };

        var options = GetRequiredService<IOptions<JsonOptions>>();

        var response = await Client.PostAsJsonAsync($"/Exam/{_exam.Id}/SetParticipants", setParticipantsModel,
            options.Value.JsonSerializerOptions);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Course does not exist.");

        var expectedExamCourse = new ExamCourse(_exam.Id, _course.Id);
        var expectedExamStudent = new ExamStudent(_exam.Id, _otherStudent.Id);

        using (var repository = GetSchoolExamRepository())
        {
            var exam = repository.Find(new ExamWithParticipantsById(_exam.Id))!;
            exam.Participants.Should().HaveCount(2);
            exam.Participants.Should().ContainEquivalentOf(expectedExamCourse,
                x => x.Including(e => e.ExamId).Including(e => e.ParticipantId));
            exam.Participants.Should().ContainEquivalentOf(expectedExamStudent,
                x => x.Including(e => e.ExamId).Including(e => e.ParticipantId));
        }
    }

    [Test]
    public async Task ExamController_SetParticipants_StudentDoesNotExist_ThrowsException()
    {
        await ResetExam();

        var course = TestEntityFactory.Create<Course>();
        using (var repository = GetSchoolExamRepository())
        {
            repository.Add(course);
            await repository.SaveChangesAsync();
        }

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var setParticipantsModel = new SetParticipantsModel
        {
            Participants = new ExamParticipantWriteModel[]
            {
                new ExamCourseWriteModel {Id = course.Id},
                new ExamStudentWriteModel {Id = Guid.NewGuid()}
            }
        };

        var options = GetRequiredService<IOptions<JsonOptions>>();

        var response = await Client.PostAsJsonAsync($"/Exam/{_exam.Id}/SetParticipants", setParticipantsModel,
            options.Value.JsonSerializerOptions);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Student does not exist.");

        var expectedExamCourse = new ExamCourse(_exam.Id, _course.Id);
        var expectedExamStudent = new ExamStudent(_exam.Id, _otherStudent.Id);

        using (var repository = GetSchoolExamRepository())
        {
            var exam = repository.Find(new ExamWithParticipantsById(_exam.Id))!;
            exam.Participants.Should().HaveCount(2);
            exam.Participants.Should().ContainEquivalentOf(expectedExamCourse,
                x => x.Including(e => e.ExamId).Including(e => e.ParticipantId));
            exam.Participants.Should().ContainEquivalentOf(expectedExamStudent,
                x => x.Including(e => e.ExamId).Including(e => e.ParticipantId));
        }
    }

    [Test]
    public async Task ExamController_SetGradingTable_ExamCreator_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var setParticipantsModel = new GradingTableWriteModel
        {
            LowerBounds = new GradingTableLowerBoundModelBase[]
            {
                new GradingTableLowerBoundPointsModel {Grade = "Excellent", Points = 16},
                new GradingTableLowerBoundPointsModel {Grade = "Good", Points = 12},
                new GradingTableLowerBoundPercentageModel {Grade = "Sufficient", Percentage = 40},
                new GradingTableLowerBoundPointsModel {Grade = "Bad", Points = 0}
            }
        };

        var options = GetRequiredService<IOptions<JsonOptions>>();

        var response = await Client.PostAsJsonAsync($"/Exam/{_exam.Id}/SetGradingTable", setParticipantsModel,
            options.Value.JsonSerializerOptions);
        response.EnsureSuccessStatusCode();

        var expectedGradingTable = new GradingTable(Guid.Empty, _exam.Id)
        {
            Intervals = new List<GradingTableInterval>
            {
                new(new GradingTableIntervalBound(0, GradingTableIntervalBoundType.Inclusive),
                    new GradingTableIntervalBound(8, GradingTableIntervalBoundType.Exclusive), "Bad",
                    GradingTableLowerBoundType.Points, Guid.Empty),
                new(new GradingTableIntervalBound(8, GradingTableIntervalBoundType.Inclusive),
                    new GradingTableIntervalBound(12, GradingTableIntervalBoundType.Exclusive), "Sufficient",
                    GradingTableLowerBoundType.Percentage, Guid.Empty),
                new(new GradingTableIntervalBound(12, GradingTableIntervalBoundType.Inclusive),
                    new GradingTableIntervalBound(16, GradingTableIntervalBoundType.Exclusive), "Good",
                    GradingTableLowerBoundType.Points, Guid.Empty),
                new(new GradingTableIntervalBound(16, GradingTableIntervalBoundType.Inclusive),
                    new GradingTableIntervalBound(20, GradingTableIntervalBoundType.Inclusive), "Excellent",
                    GradingTableLowerBoundType.Points, Guid.Empty)
            }
        };

        using (var repository = GetSchoolExamRepository())
        {
            var exam = repository.Find(new ExamWithGradingTableById(_exam.Id))!;
            var gradingTable = exam.GradingTable!;
            gradingTable.Intervals.Should().HaveCount(4);
            gradingTable.Intervals.OrderBy(x => x.Start).Should().BeEquivalentTo(expectedGradingTable.Intervals);
        }
    }

    [Test]
    public async Task ExamController_SetGradingTable_NoTaskPdf_ThrowsException()
    {
        await ResetExam();
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

        var setParticipantsModel = new GradingTableWriteModel
        {
            LowerBounds = new GradingTableLowerBoundModelBase[]
            {
                new GradingTableLowerBoundPointsModel {Grade = "Excellent", Points = 16},
                new GradingTableLowerBoundPointsModel {Grade = "Good", Points = 12},
                new GradingTableLowerBoundPercentageModel {Grade = "Sufficient", Percentage = 40},
                new GradingTableLowerBoundPointsModel {Grade = "Bad", Points = 0}
            }
        };

        var options = GetRequiredService<IOptions<JsonOptions>>();

        var response = await Client.PostAsJsonAsync($"/Exam/{_exam.Id}/SetGradingTable", setParticipantsModel,
            options.Value.JsonSerializerOptions);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("An exam without a task PDF file cannot have a grading table.");
    }

    [Test]
    public async Task ExamController_SetGradingTable_NoIntervals_ThrowsException()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var setParticipantsModel = new GradingTableWriteModel
        {
            LowerBounds = new GradingTableLowerBoundModelBase[] { }
        };

        var options = GetRequiredService<IOptions<JsonOptions>>();

        var response = await Client.PostAsJsonAsync($"/Exam/{_exam.Id}/SetGradingTable", setParticipantsModel,
            options.Value.JsonSerializerOptions);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Grading table must contain at least one interval.");
    }
    
    [Test]
    public async Task ExamController_SetGradingTable_ZeroIntervalMissing_ThrowsException()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var setParticipantsModel = new GradingTableWriteModel
        {
            LowerBounds = new GradingTableLowerBoundModelBase[]
            {
                new GradingTableLowerBoundPointsModel {Grade = "Excellent", Points = 16},
                new GradingTableLowerBoundPointsModel {Grade = "Good", Points = 12},
                new GradingTableLowerBoundPercentageModel {Grade = "Sufficient", Percentage = 40}
            }
        };

        var options = GetRequiredService<IOptions<JsonOptions>>();

        var response = await Client.PostAsJsonAsync($"/Exam/{_exam.Id}/SetGradingTable", setParticipantsModel,
            options.Value.JsonSerializerOptions);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("A grading interval starting from 0.0 points must be included.");
    }
    
    [Test]
    public async Task ExamController_SetGradingTable_IntervalHasMoreThanExamMaximumPoints_ThrowsException()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var setParticipantsModel = new GradingTableWriteModel
        {
            LowerBounds = new GradingTableLowerBoundModelBase[]
            {
                new GradingTableLowerBoundPointsModel {Grade = "Better than excellent", Points = 22},
                new GradingTableLowerBoundPointsModel {Grade = "Excellent", Points = 16},
                new GradingTableLowerBoundPointsModel {Grade = "Good", Points = 12},
                new GradingTableLowerBoundPercentageModel {Grade = "Sufficient", Percentage = 40},
                new GradingTableLowerBoundPointsModel {Grade = "Bad", Points = 0}
            }
        };

        var options = GetRequiredService<IOptions<JsonOptions>>();

        var response = await Client.PostAsJsonAsync($"/Exam/{_exam.Id}/SetGradingTable", setParticipantsModel,
            options.Value.JsonSerializerOptions);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("A grading interval exceeds the maximum number of points.");
    }
    
    [Test]
    public async Task ExamController_SetGradingTable_EmptyInterval_ThrowsException()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var setParticipantsModel = new GradingTableWriteModel
        {
            LowerBounds = new GradingTableLowerBoundModelBase[]
            {
                new GradingTableLowerBoundPointsModel {Grade = "Excellent", Points = 12},
                new GradingTableLowerBoundPointsModel {Grade = "Good", Points = 12},
                new GradingTableLowerBoundPercentageModel {Grade = "Sufficient", Percentage = 40},
                new GradingTableLowerBoundPointsModel {Grade = "Bad", Points = 0}
            }
        };

        var options = GetRequiredService<IOptions<JsonOptions>>();

        var response = await Client.PostAsJsonAsync($"/Exam/{_exam.Id}/SetGradingTable", setParticipantsModel,
            options.Value.JsonSerializerOptions);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("A grading interval must not be empty.");
    }

    [Test]
    public async Task ExamController_UploadTaskPdf_ExamCreator_Success()
    {
        await ResetExam();

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var taskIds = new[] {Guid.NewGuid(), Guid.NewGuid()};
        var taskPdf = CreateTaskPdfFile(taskIds);
        var uploadTaskPdfModel = new UploadTaskPdfModel
        {
            TaskPdf = Convert.ToBase64String(taskPdf),
            Tasks = new[]
            {
                new ExamTaskWriteModel {Id = taskIds[0], Title = "Task 1", MaxPoints = 5},
                new ExamTaskWriteModel {Id = taskIds[1], Title = "Task 2", MaxPoints = 10}
            }
        };

        var response = await Client.PostAsJsonAsync($"/Exam/{_exam.Id}/UploadTaskPdf", uploadTaskPdfModel);
        response.EnsureSuccessStatusCode();

        var expectedTask1 = new ExamTask(taskIds[0], "Task 1", 5, _exam.Id, new ExamPosition(1, 0),
            new ExamPosition(1, 0));
        var expectedTask2 = new ExamTask(taskIds[0], "Task 2", 10, _exam.Id, new ExamPosition(1, 0),
            new ExamPosition(1, 0));

        EquivalencyAssertionOptions<ExamTask>
            ConfigExamTaskEquivalencyAssertionOptions(EquivalencyAssertionOptions<ExamTask> x) =>
            x.Excluding(t => t.Id).Excluding(t => t.Start).Excluding(t => t.End);

        using var repository = GetSchoolExamRepository();
        var exam = repository.Find(new ExamWithTaskPdfFileAndTasksByIdSpecification(_exam.Id))!;
        exam.Tasks.Should().HaveCount(2);
        exam.Tasks.Should().ContainEquivalentOf(expectedTask1,
            (Func<EquivalencyAssertionOptions<ExamTask>, EquivalencyAssertionOptions<ExamTask>>)
            ConfigExamTaskEquivalencyAssertionOptions);
        exam.Tasks.Should().ContainEquivalentOf(expectedTask2,
            (Func<EquivalencyAssertionOptions<ExamTask>, EquivalencyAssertionOptions<ExamTask>>)
            ConfigExamTaskEquivalencyAssertionOptions);
    }

    [Test]
    public async Task ExamController_UploadTaskPdf_CallWithSamePdfTwice_Success()
    {
        await ResetExam();

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var taskIds = new[] {Guid.NewGuid(), Guid.NewGuid()};
        var taskPdf = CreateTaskPdfFile(taskIds);
        var uploadTaskPdfModel = new UploadTaskPdfModel
        {
            TaskPdf = Convert.ToBase64String(taskPdf),
            Tasks = new[]
            {
                new ExamTaskWriteModel {Id = taskIds[0], Title = "Task 1", MaxPoints = 5},
                new ExamTaskWriteModel {Id = taskIds[1], Title = "Task 2", MaxPoints = 10}
            }
        };

        var response1 = await Client.PostAsJsonAsync($"/Exam/{_exam.Id}/UploadTaskPdf", uploadTaskPdfModel);
        response1.EnsureSuccessStatusCode();

        var response2 = await Client.PostAsJsonAsync($"/Exam/{_exam.Id}/UploadTaskPdf", uploadTaskPdfModel);
        response2.EnsureSuccessStatusCode();

        var expectedTask1 = new ExamTask(taskIds[0], "Task 1", 5, _exam.Id, new ExamPosition(1, 0),
            new ExamPosition(1, 0));
        var expectedTask2 = new ExamTask(taskIds[0], "Task 2", 10, _exam.Id, new ExamPosition(1, 0),
            new ExamPosition(1, 0));

        EquivalencyAssertionOptions<ExamTask>
            ConfigExamTaskEquivalencyAssertionOptions(EquivalencyAssertionOptions<ExamTask> x) =>
            x.Excluding(t => t.Id).Excluding(t => t.Start).Excluding(t => t.End);

        using var repository = GetSchoolExamRepository();
        var exam = repository.Find(new ExamWithTaskPdfFileAndTasksByIdSpecification(_exam.Id))!;
        exam.Tasks.Should().HaveCount(2);
        exam.Tasks.Should().ContainEquivalentOf(expectedTask1,
            (Func<EquivalencyAssertionOptions<ExamTask>, EquivalencyAssertionOptions<ExamTask>>)
            ConfigExamTaskEquivalencyAssertionOptions);
        exam.Tasks.Should().ContainEquivalentOf(expectedTask2,
            (Func<EquivalencyAssertionOptions<ExamTask>, EquivalencyAssertionOptions<ExamTask>>)
            ConfigExamTaskEquivalencyAssertionOptions);
    }

    [Test]
    public async Task ExamController_UploadTaskPdf_AdditionalTaskMarkers_AdditionalMarkersIgnored()
    {
        await ResetExam();

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var taskIds = new[] {Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()};
        var taskPdf = CreateTaskPdfFile(taskIds);
        var uploadTaskPdfModel = new UploadTaskPdfModel
        {
            TaskPdf = Convert.ToBase64String(taskPdf),
            Tasks = new[]
            {
                new ExamTaskWriteModel {Id = taskIds[0], Title = "Task 1", MaxPoints = 5},
                new ExamTaskWriteModel {Id = taskIds[1], Title = "Task 2", MaxPoints = 10}
            }
        };

        var response = await Client.PostAsJsonAsync($"/Exam/{_exam.Id}/UploadTaskPdf", uploadTaskPdfModel);
        response.EnsureSuccessStatusCode();

        var expectedTask1 = new ExamTask(taskIds[0], "Task 1", 5, _exam.Id, new ExamPosition(1, 0),
            new ExamPosition(1, 0));
        var expectedTask2 = new ExamTask(taskIds[0], "Task 2", 10, _exam.Id, new ExamPosition(1, 0),
            new ExamPosition(1, 0));

        EquivalencyAssertionOptions<ExamTask>
            ConfigExamTaskEquivalencyAssertionOptions(EquivalencyAssertionOptions<ExamTask> x) =>
            x.Excluding(t => t.Id).Excluding(t => t.Start).Excluding(t => t.End);

        using var repository = GetSchoolExamRepository();
        var exam = repository.Find(new ExamWithTaskPdfFileAndTasksByIdSpecification(_exam.Id))!;
        exam.Tasks.Should().HaveCount(2);
        exam.Tasks.Should().ContainEquivalentOf(expectedTask1,
            (Func<EquivalencyAssertionOptions<ExamTask>, EquivalencyAssertionOptions<ExamTask>>)
            ConfigExamTaskEquivalencyAssertionOptions);
        exam.Tasks.Should().ContainEquivalentOf(expectedTask2,
            (Func<EquivalencyAssertionOptions<ExamTask>, EquivalencyAssertionOptions<ExamTask>>)
            ConfigExamTaskEquivalencyAssertionOptions);
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

        var response = await Client.PostAsJsonAsync($"/Exam/{_exam.Id}/UploadTaskPdf", uploadTaskPdfModel);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("The task PDF file of an exam that already has been built cannot be changed.");
    }

    [Test]
    public async Task ExamController_UploadTaskPdf_TaskWithNegativePoints_ThrowsException()
    {
        await ResetExam();

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var taskIds = new[] {Guid.NewGuid(), Guid.NewGuid()};
        var taskPdf = CreateTaskPdfFile(taskIds);
        var uploadTaskPdfModel = new UploadTaskPdfModel
        {
            TaskPdf = Convert.ToBase64String(taskPdf),
            Tasks = new[]
            {
                new ExamTaskWriteModel {Id = taskIds[0], Title = "Task 1", MaxPoints = 5},
                new ExamTaskWriteModel {Id = taskIds[1], Title = "Task 2", MaxPoints = -10}
            }
        };

        var response = await this.Client.PostAsJsonAsync($"/Exam/{_exam.Id}/UploadTaskPdf", uploadTaskPdfModel);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Maximum number of points must be a positive number.");
    }

    [Test]
    public async Task ExamController_UploadTaskPdf_SameTaskMarkedTwice_ThrowsException()
    {
        await ResetExam();

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var taskIds = new[] {Guid.NewGuid(), Guid.NewGuid()};
        var taskPdf = CreateTaskPdfFile(new[] {taskIds[0], taskIds[0], taskIds[1]});
        var pdfService = GetRequiredService<IPdfService>();
        var annotations = pdfService.GetUriLinkAnnotations(taskPdf).ToList();
        var annotationToRemove = annotations.First();
        var duplicateAnnotation = annotations.GroupBy(x => x.Uri)
            .First(x => x.Count() > 1 && !x.Key.Equals(annotationToRemove.Uri));
        taskPdf = pdfService.RemoveUriLinkAnnotations(taskPdf, annotationToRemove);
        var uploadTaskPdfModel = new UploadTaskPdfModel
        {
            TaskPdf = Convert.ToBase64String(taskPdf),
            Tasks = new[]
            {
                new ExamTaskWriteModel {Id = taskIds[0], Title = "Task 1", MaxPoints = 5},
                new ExamTaskWriteModel {Id = taskIds[1], Title = "Task 2", MaxPoints = 10}
            }
        };

        var response = await Client.PostAsJsonAsync($"/Exam/{_exam.Id}/UploadTaskPdf", uploadTaskPdfModel);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain($"Task marker with text {duplicateAnnotation.Key} was found in PDF more than once.");
    }

    [Test]
    public async Task ExamController_UploadTaskPdf_TaskEndMarkerMissing_ThrowsException()
    {
        await ResetExam();

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var taskIds = new[] {Guid.NewGuid(), Guid.NewGuid()};
        var taskPdf = CreateTaskPdfFile(new[] {taskIds[0], taskIds[1]});
        var pdfService = GetRequiredService<IPdfService>();
        var annotations = pdfService.GetUriLinkAnnotations(taskPdf).ToList();
        var annotationToRemove = annotations.First(x => x.Uri.Equals($"task-end-{taskIds[1]}"));
        taskPdf = pdfService.RemoveUriLinkAnnotations(taskPdf, annotationToRemove);
        var uploadTaskPdfModel = new UploadTaskPdfModel
        {
            TaskPdf = Convert.ToBase64String(taskPdf),
            Tasks = new[]
            {
                new ExamTaskWriteModel {Id = taskIds[0], Title = "Task 1", MaxPoints = 5},
                new ExamTaskWriteModel {Id = taskIds[1], Title = "Task 2", MaxPoints = 10}
            }
        };

        var response = await Client.PostAsJsonAsync($"/Exam/{_exam.Id}/UploadTaskPdf", uploadTaskPdfModel);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain($"No end marker was found for task with id {taskIds[1]}.");
    }

    [Test]
    public async Task ExamController_UploadTaskPdf_MarkersOfTaskMissing_ThrowsException()
    {
        await ResetExam();

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var taskIds = new[] {Guid.NewGuid(), Guid.NewGuid()};
        var taskPdf = CreateTaskPdfFile(new[] {taskIds[0]});
        var uploadTaskPdfModel = new UploadTaskPdfModel
        {
            TaskPdf = Convert.ToBase64String(taskPdf),
            Tasks = new[]
            {
                new ExamTaskWriteModel {Id = taskIds[0], Title = "Task 1", MaxPoints = 5},
                new ExamTaskWriteModel {Id = taskIds[1], Title = "Task 2", MaxPoints = 10}
            }
        };

        var response = await Client.PostAsJsonAsync($"/Exam/{_exam.Id}/UploadTaskPdf", uploadTaskPdfModel);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain($"Task with id {taskIds[1]} could not be found in PDF.");
    }

    [Test]
    public async Task ExamController_UploadTaskPdf_NoTasks_ThrowsException()
    {
        await ResetExam();

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var taskIds = new[] {Guid.NewGuid(), Guid.NewGuid()};
        var taskPdf = CreateTaskPdfFile(taskIds);
        var uploadTaskPdfModel = new UploadTaskPdfModel
            {TaskPdf = Convert.ToBase64String(taskPdf), Tasks = new List<ExamTaskWriteModel>()};

        var response = await Client.PostAsJsonAsync($"/Exam/{_exam.Id}/UploadTaskPdf", uploadTaskPdfModel);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("An exam must contain at least one task.");
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
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("The exam has not been built yet.");
    }

    [Test]
    public async Task ExamController_Clean_ExamWithSubmissionPages_ThrowsException()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var response = await this.Client.PostAsync($"/Exam/{_exam.Id}/Clean", null);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("An exam with existing submission pages must not be cleaned.");
    }
    

    [Test]
    public async Task ExamController_BuildAndSubmit_ExamCreator_Success()
    {
        // remove submission pages such that a rebuild is possible
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

        var buildResponse = await Client.PostAsync($"/Exam/{_exam.Id}/Build", null);
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
            // merge PDFs
            var pdfs = booklets?.Select(x => x.PdfFile.Content).ToArray();
            submissionPdf = pdfService.Merge(pdfs ?? Array.Empty<byte[]>());
            
            // add student QR codes
            var qrCodeGenerator = GetRequiredService<IQrCodeGenerator>();
            var pdfImageRenderInfo =
                new PdfImageRenderInfo(1, 200, 200, 100, qrCodeGenerator.GeneratePngQrCode(_student.QrCode.Data));
            var otherPdfImageRenderInfo = new PdfImageRenderInfo(3, 200, 200, 100,
                qrCodeGenerator.GeneratePngQrCode(_otherStudent.QrCode.Data));
            submissionPdf = pdfService.RenderImages(submissionPdf, pdfImageRenderInfo, otherPdfImageRenderInfo);
        }

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var uploadSubmissionsModel = new UploadSubmissionsModel {Pdf = Convert.ToBase64String(submissionPdf)};

        var response = await Client.PostAsJsonAsync($"/Submission/Upload/{_exam.Id}", uploadSubmissionsModel);
        response.EnsureSuccessStatusCode();

        using (var repository = GetSchoolExamRepository())
        {
            var exam = repository.Find<Exam>(_exam.Id);
            exam?.State.Should().Be(ExamState.InCorrection);
            var pages = repository.List(new SubmissionPageByExamSpecification(_exam.Id)).ToList();
            pages.Should().HaveCount(4);
            pages.Select(x => x.SubmissionId.HasValue).Should().AllBeEquivalentTo(true);
            var submissionIds = pages.Select(x => x.SubmissionId!.Value).ToHashSet();
            var submissions = repository.List(new SubmissionWithPdfFileByIdsSpecification(submissionIds)).ToList();
            submissions.Select(x => x.PdfFile != null).Should().AllBeEquivalentTo(true);
            var matchedStudentIds = submissions.Select(x => x.StudentId).ToList();
            matchedStudentIds.Should().Contain(_student.Id);
            matchedStudentIds.Should().Contain(_otherStudent.Id);
        }
    }

    [Test]
    public async Task ExamController_BuildAndSubmit_ConcatSameSubmissionPdfTwice_Success()
    {
        await ResetExam();

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var buildResponse = await Client.PostAsync($"/Exam/{_exam.Id}/Build", null);
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

        var uploadSubmissionsModel = new UploadSubmissionsModel {Pdf = Convert.ToBase64String(submissionPdf)};

        var response = await Client.PostAsJsonAsync($"/Submission/Upload/{_exam.Id}", uploadSubmissionsModel);

        response.EnsureSuccessStatusCode();

        using (var repository = GetSchoolExamRepository())
        {
            var exam = repository.Find<Exam>(_exam.Id);
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
    public async Task ExamController_BuildAndSubmit_StudentsMatchedToSameBooklet_ThrowsException()
    {
        await ResetExam();

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var buildResponse = await Client.PostAsync($"/Exam/{_exam.Id}/Build", null);
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
            // merge PDFs
            var pdfs = booklets?.Select(x => x.PdfFile.Content).ToArray();
            submissionPdf = pdfService.Merge(pdfs ?? Array.Empty<byte[]>());
            
            // add student QR codes
            var qrCodeGenerator = GetRequiredService<IQrCodeGenerator>();
            var pdfImageRenderInfo =
                new PdfImageRenderInfo(1, 200, 200, 100, qrCodeGenerator.GeneratePngQrCode(_student.QrCode.Data));
            var otherPdfImageRenderInfo = new PdfImageRenderInfo(2, 200, 200, 100,
                qrCodeGenerator.GeneratePngQrCode(_otherStudent.QrCode.Data));
            submissionPdf = pdfService.RenderImages(submissionPdf, pdfImageRenderInfo, otherPdfImageRenderInfo);
        }

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var uploadSubmissionsModel = new UploadSubmissionsModel {Pdf = Convert.ToBase64String(submissionPdf)};

        var response = await Client.PostAsJsonAsync($"/Submission/Upload/{_exam.Id}", uploadSubmissionsModel);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should()
            .Contain(
                $"Submission cannot be assigned to student with identifier {_otherStudent.Id} because it was previously assigned to student with identifier {_student.Id}.");
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

        var response = await Client.PostAsync($"/Exam/{_exam.Id}/Build", null);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("At least one exam booklet must be built.");
    }

    [Test]
    public async Task ExamController_Build_TaskPdfFileMissing_ThrowsException()
    {
        await ResetExam();
        using (var repository = GetSchoolExamRepository())
        {
            var exam = repository.Find(new ExamWithTaskPdfFileAndGradingTableByIdSpecification(_exam.Id))!;
            repository.Remove(exam.TaskPdfFile!);
            await repository.SaveChangesAsync();
        }

        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var response = await Client.PostAsync($"/Exam/{_exam.Id}/Build", null);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Exam does not have a task PDF file.");
    }

    [Test]
    public async Task ExamController_Build_ExamWithSubmissionPages_ThrowsException()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var response = await Client.PostAsync($"/Exam/{_exam.Id}/Build", null);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("An exam with existing submission pages must not be cleaned.");
    }

    [Test]
    public async Task ExamController_GetUnmatchedPages_ExamCreator_Success()
    {
        SetClaims(new Claim(ClaimTypes.Role, Role.Teacher),
            new Claim(CustomClaimTypes.PersonId, _teacher.Id.ToString()),
            new Claim(CustomClaimTypes.UserId, _user.Id.ToString()));

        var response = await Client.GetAsync($"/Exam/{_exam.Id}/UnmatchedPages");
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

        var response = await Client.GetAsync($"/Exam/{examId}/UnmatchedPages");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
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

        var response = await Client.PostAsJsonAsync($"/Exam/{_exam.Id}/MatchPages", manualMatchesModel);
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

        var response = await Client.PostAsJsonAsync($"/Exam/{_exam.Id}/MatchPages", manualMatchesModel);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
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

        var response = await Client.PostAsJsonAsync($"/Exam/{_exam.Id}/MatchPages", manualMatchesModel);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
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

        var response = await Client.PostAsJsonAsync($"/Exam/{_exam.Id}/MatchPages", manualMatchesModel);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
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

        var response = await Client.PostAsJsonAsync($"/Exam/{_exam.Id}/MatchPages", manualMatchesModel);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
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

        var response = await Client.PostAsJsonAsync($"/Exam/{_exam.Id}/MatchPages", manualMatchesModel);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
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

        var response = await Client.PostAsJsonAsync($"/Exam/{_exam.Id}/MatchPages", manualMatchesModel);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
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

        var response = await Client.PostAsJsonAsync($"/Exam/{_exam.Id}/MatchPages", manualMatchesModel);
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

        var exam = repository.Find<Exam>(_exam.Id)!;
        exam.State = ExamState.BuildReady;
        repository.Update(exam);

        await repository.SaveChangesAsync();
    }

    private byte[] CreateTaskPdfFile(Guid[] taskIds)
    {
        var stream = new MemoryStream();
        var writer = new PdfWriter(stream);
        var pdf = new PdfDocument(writer);
        var document = new Document(pdf);

        for (int i = 0; i < taskIds.Length; i++)
        {
            var taskId = taskIds[i];
            var actionStart = PdfAction.CreateURI($"task-start-{taskId}");
            var linkStart = new Link("Start", actionStart);
            var text = $"This is task number {i} of a test exam";
            document.Add(new Paragraph().Add(linkStart).Add(text));
            var actionEnd = PdfAction.CreateURI($"task-end-{taskId}");
            var linkEnd = new Link("End", actionEnd);
            document.Add(new Paragraph().Add(linkEnd));
            if (i < taskIds.Length - 1)
            {
                document.Add(new AreaBreak());
            }
        }

        document.Close();

        var result = stream.ToArray();

        return result;
    }
}