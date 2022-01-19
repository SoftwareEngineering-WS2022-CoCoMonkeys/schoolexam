using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolExam.Application.Services;
using SchoolExam.Application.TagLayout;
using SchoolExam.Domain.Exceptions;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Extensions;
using SchoolExam.Web.Authorization;
using SchoolExam.Web.Models;
using SchoolExam.Web.Models.Exam;

namespace SchoolExam.Web.Controllers;

public class ExamController : ApiController<ExamController>
{
    private readonly IExamService _examService;

    public ExamController(ILogger<ExamController> logger, IMapper mapper, IExamService examService) : base(logger,
        mapper)
    {
        _examService = examService;
    }

    [HttpGet]
    [Route("ByTeacher")]
    [Authorize(Roles = Role.TeacherName)]
    public IEnumerable<ExamReadModelTeacher> GetByTeacher()
    {
        var exams = _examService.GetByTeacher(GetPersonId()!.Value);
        return Mapper.Map<IEnumerable<ExamReadModelTeacher>>(exams);
    }

    [HttpPost]
    [Route($"Create")]
    [Authorize(Roles = Role.TeacherName)]
    public async Task<IdReadModel> Create([FromBody] ExamWriteModel examWriteModel)
    {
        var examId = await _examService.Create(examWriteModel.Title, examWriteModel.Date.SetKindUtc(),
            GetPersonId()!.Value, examWriteModel.Topic);
        var idReadModel = new IdReadModel {Id = examId};
        return idReadModel;
    }

    [HttpPut]
    [Route($"{{{RouteParameterNames.ExamIdParameterName}}}/Update")]
    [Authorize(PolicyNames.ExamCreatorPolicyName)]
    public async Task<IActionResult> Update(Guid examId, [FromBody] ExamWriteModel examWriteModel)
    {
        await _examService.Update(examId, examWriteModel.Title, examWriteModel.Date.SetKindUtc());
        return Ok();
    }

    [HttpDelete]
    [Route($"{{{RouteParameterNames.ExamIdParameterName}}}/Delete")]
    [Authorize(PolicyNames.ExamCreatorPolicyName)]
    public async Task<IActionResult> Delete(Guid examId)
    {
        await _examService.Delete(examId);
        return Ok();
    }

    [HttpPost]
    [Route($"{{{RouteParameterNames.ExamIdParameterName}}}/SetParticipants")]
    [Authorize(PolicyNames.ExamCreatorPolicyName)]
    public async Task<IActionResult> SetParticipants(Guid examId, [FromBody] SetParticipantsModel setParticipantsModel)
    {
        var courseIds = setParticipantsModel.Participants.OfType<ExamCourseWriteModel>().Select(x => x.Id);
        var studentIds = setParticipantsModel.Participants.OfType<ExamStudentWriteModel>().Select(x => x.Id);
        await _examService.SetParticipants(examId, courseIds, studentIds);
        return Ok();
    }

    [HttpPost]
    [Route($"{{{RouteParameterNames.ExamIdParameterName}}}/UploadTaskPdf")]
    [Authorize(PolicyNames.ExamCreatorPolicyName)]
    public async Task<IActionResult> UploadTaskPdf(Guid examId, [FromBody] UploadTaskPdfModel uploadTaskPdfModel)
    {
        var pdf = Convert.FromBase64String(uploadTaskPdfModel.TaskPdf);

        var tasks = Mapper.Map<IEnumerable<ExamTaskInfo>>(uploadTaskPdfModel.Tasks).ToArray();
        await _examService.SetTaskPdfFile(examId, GetUserId()!.Value, pdf, tasks);

        return Ok();
    }

    [HttpPost]
    [Route($"{{{RouteParameterNames.ExamIdParameterName}}}/Build")]
    [Authorize(PolicyNames.ExamCreatorPolicyName)]
    public async Task<BuildResultModel> Build(Guid examId)
    {
        var count = await _examService.Build(examId, GetUserId()!.Value);
        var pdf = _examService.GetConcatenatedBookletPdfFile(examId);
        var qrCodePdf = _examService.GetParticipantQrCodePdf<AveryZweckform3475200>(examId);
        return new BuildResultModel
            {Count = count, PdfFile = Convert.ToBase64String(pdf), QrCodePdfFile = Convert.ToBase64String(qrCodePdf)};
    }

    [HttpPost]
    [Route($"{{{RouteParameterNames.ExamIdParameterName}}}/Clean")]
    [Authorize(PolicyNames.ExamCreatorPolicyName)]
    public async Task<IActionResult> Clean(Guid examId)
    {
        await _examService.Clean(examId);
        return Ok();
    }

    [HttpGet]
    [Route($"{{{RouteParameterNames.ExamIdParameterName}}}/UnmatchedPages")]
    [Authorize(PolicyNames.ExamCreatorPolicyName)]
    public UnmatchedPagesReadModel GetUnmatchedPages(Guid examId)
    {
        var unmatchedBookletPages = _examService.GetUnmatchedBookletPages(examId);
        var unmatchedBookletPagesMapped = Mapper.Map<IEnumerable<UnmatchedBookletPageReadModel>>(unmatchedBookletPages);

        var unmatchedSubmissionPages = _examService.GetUnmatchedSubmissionPages(examId);
        var unmatchedSubmissionPagesMapped =
            Mapper.Map<IEnumerable<UnmatchedSubmissionPageReadModel>>(unmatchedSubmissionPages);

        return new UnmatchedPagesReadModel
        {
            UnmatchedBookletPages = unmatchedBookletPagesMapped,
            UnmatchedSubmissionPages = unmatchedSubmissionPagesMapped
        };
    }

    [HttpPost]
    [Route($"{{{RouteParameterNames.ExamIdParameterName}}}/MatchPages")]
    [Authorize(PolicyNames.ExamCreatorPolicyName)]
    public async Task<IActionResult> MatchManually(Guid examId, [FromBody] ManualMatchesModel manualMatchesModel)
    {
        foreach (var manualMatchModel in manualMatchesModel.Matches)
        {
            await _examService.MatchManually(examId, manualMatchModel.BookletPageId,
                manualMatchModel.SubmissionPageId, GetUserId()!.Value);
        }

        return Ok();
    }

    [HttpPost]
    [Route($"{{{RouteParameterNames.ExamIdParameterName}}}/Publish")]
    [Authorize(PolicyNames.ExamCreatorPolicyName)]
    public async Task<IActionResult> Publish(Guid examId, [FromBody] PublishExamWriteModel publishExamWriteModel)
    {
        await _examService.Publish(examId, publishExamWriteModel.PublishingDateTime);
        return Ok();
    }

    [HttpPost]
    [Route($"{{{RouteParameterNames.ExamIdParameterName}}}/SetGradingTable")]
    [Authorize(PolicyNames.ExamCreatorPolicyName)]
    public async Task<IActionResult> SetGradingTable(Guid examId,
        [FromBody] GradingTableWriteModel gradingTableWriteModel)
    {
        var maxPoints = _examService.GetMaxPoints(examId);

        double GetPoints(GradingTableLowerBoundModelBase lowerBound)
        {
            if (lowerBound is GradingTableLowerBoundPointsModel lowerBoundPoints)
            {
                return lowerBoundPoints.Points;
            }

            if (lowerBound is GradingTableLowerBoundPercentageModel lowerBoundPercentage)
            {
                return lowerBoundPercentage.Percentage / 100.0 * maxPoints;
            }

            throw new DomainException("Invalid type for grading table lower bound");
        }

        GradingTableLowerBoundType GetType(GradingTableLowerBoundModelBase lowerBound)
        {
            if (lowerBound is GradingTableLowerBoundPointsModel)
            {
                return GradingTableLowerBoundType.Points;
            }

            if (lowerBound is GradingTableLowerBoundPercentageModel)
            {
                return GradingTableLowerBoundType.Percentage;
            }

            throw new DomainException("Invalid type for grading table lower bound");
        }

        var lowerBounds =
            gradingTableWriteModel.LowerBounds.Select(x =>
                new GradingTableIntervalLowerBound(GetPoints(x), GetType(x), x.Grade));

        await _examService.SetGradingTable(examId, lowerBounds.ToArray());

        return Ok();
    }
}