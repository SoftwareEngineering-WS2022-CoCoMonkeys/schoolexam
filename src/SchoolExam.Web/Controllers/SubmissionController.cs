using System.Net.Mime;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolExam.Application.Services;
using SchoolExam.Web.Authorization;
using SchoolExam.Web.Models.Submission;

namespace SchoolExam.Web.Controllers;

public class SubmissionController : ApiController<SubmissionController>
{
    private readonly ISubmissionService _submissionService;
    private readonly IExamService _examService;

    public SubmissionController(ILogger<SubmissionController> logger, IMapper mapper,
        ISubmissionService submissionService, IExamService examService) : base(logger, mapper)
    {
        _submissionService = submissionService;
        _examService = examService;
    }
    
    [HttpPost]
    [Route($"Upload/{{{RouteParameterNames.ExamIdParameterName}}}")]
    [Authorize(PolicyNames.ExamCreatorPolicyName)]
    public async Task<IActionResult> SubmitAndMatch(Guid examId, [FromBody] UploadSubmissionsModel uploadSubmissionsModel)
    {
        var pdf = Convert.FromBase64String(uploadSubmissionsModel.Pdf);

        await _examService.Match(examId, pdf, GetUserId()!.Value);

        return Ok();
    }

    [HttpGet]
    [Route($"ByExam/{{{RouteParameterNames.ExamIdParameterName}}}")]
    [Authorize(PolicyNames.ExamCreatorPolicyName)]
    public IEnumerable<SubmissionReadModel> GetSubmissions(Guid examId)
    {
        var submissions = _submissionService.GetByExam(examId);
        var result = Mapper.Map<IEnumerable<SubmissionReadModel>>(submissions);

        return result;
    }

    [HttpGet]
    [Route($"{{{RouteParameterNames.SubmissionIdParameterName}}}/Details")]
    [Authorize(PolicyNames.SubmissionExamCreatorPolicyName)]
    public SubmissionDetailsReadModel GetSubmissionWithDetails(Guid submissionId)
    {
        var submission = _submissionService.GetByIdWithDetails(submissionId);
        var result = Mapper.Map<SubmissionDetailsReadModel>(submission);

        return result;
    }

    [HttpPost]
    [Route("ByIdsWithDetails")]
    [Authorize(PolicyNames.SubmissionsExamCreatorPolicyName)]
    public IEnumerable<SubmissionDetailsReadModel> GetSubmissionsByIdsWithDetails(
        [FromBody] SubmissionsByIdsModel submissionsByIdsModel)
    {
        var submissions = _submissionService.GetByIdsWithDetails(submissionsByIdsModel.Ids);
        var result = Mapper.Map<IEnumerable<SubmissionDetailsReadModel>>(submissions);

        return result;
    }

    [HttpGet]
    [Route($"{{{RouteParameterNames.SubmissionIdParameterName}}}/Download")]
    [Authorize(PolicyNames.SubmissionExamCreatorPolicyName)]
    public IActionResult DownloadSubmission(Guid submissionId)
    {
        var pdf = _submissionService.GetSubmissionPdf(submissionId);

        return File(pdf, MediaTypeNames.Application.Pdf);
    }
    
    [HttpGet]
    [Route($"{{{RouteParameterNames.SubmissionIdParameterName}}}/DownloadRemark")]
    [Authorize(PolicyNames.SubmissionExamCreatorPolicyName)]
    public IActionResult DownloadSubmissionRemark(Guid submissionId)
    {
        var pdf = _submissionService.GetRemarkPdf(submissionId);

        return File(pdf, MediaTypeNames.Application.Pdf);
    }

    [HttpPost]
    [Route($"{{{RouteParameterNames.SubmissionIdParameterName}}}/SetPoints")]
    [Authorize(PolicyNames.SubmissionExamCreatorPolicyName)]
    public async Task<IActionResult> SetPoints(Guid submissionId, [FromBody] SetPointsModel setPointsModel)
    {
        await _submissionService.SetPoints(submissionId, setPointsModel.TaskId, setPointsModel.AchievedPoints);

        return Ok();
    }

    [HttpPost]
    [Route($"{{{RouteParameterNames.SubmissionIdParameterName}}}/UploadRemark")]
    [Authorize(PolicyNames.SubmissionExamCreatorPolicyName)]
    public async Task<IActionResult> UploadRemark(Guid submissionId, [FromBody] UploadRemarkModel uploadRemarkModel)
    {
        var remarkPdf = Convert.FromBase64String(uploadRemarkModel.RemarkPdf);
        await _submissionService.SetRemark(submissionId, remarkPdf, GetUserId()!.Value);

        return Ok();
    }
}