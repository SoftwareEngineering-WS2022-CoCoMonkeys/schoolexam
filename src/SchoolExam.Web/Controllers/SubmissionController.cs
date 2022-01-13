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
    
    public SubmissionController(ILogger<SubmissionController> logger, IMapper mapper, ISubmissionService submissionService) : base(logger, mapper)
    {
        _submissionService = submissionService;
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
    
    [HttpGet]
    [Route($"{{{RouteParameterNames.SubmissionIdParameterName}}}/Download")]
    [Authorize(PolicyNames.SubmissionExamCreatorPolicyName)]
    public IActionResult DownloadSubmission(Guid submissionId)
    {
        var pdf = _submissionService.GetSubmissionPdf(submissionId);

        return File(pdf, MediaTypeNames.Application.Pdf);
    }
}