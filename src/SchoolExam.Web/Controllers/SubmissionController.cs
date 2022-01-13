using System.Net.Mime;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolExam.Application.Services;
using SchoolExam.Web.Authorization;
using SchoolExam.Web.Models.Exam;

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
    [Route($"{{{RouteParameterNames.SubmissionIdParameterName}}}")]
    [Authorize(PolicyNames.SubmissionExamCreatorPolicyName)]
    public IActionResult GetSubmission(Guid submissionId)
    {
        var result = _submissionService.GetSubmissionPdf(submissionId);
        return File(result, MediaTypeNames.Application.Pdf);
    }
}