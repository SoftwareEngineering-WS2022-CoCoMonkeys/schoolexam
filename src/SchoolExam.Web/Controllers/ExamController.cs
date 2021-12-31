using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolExam.Application.Repositories;
using SchoolExam.Web.Models.Exam;

namespace SchoolExam.Web.Controllers;

public class ExamController : ApiController<ExamController>
{
    public const string ExamIdParameterName = "examId";
    public const string ExamCreatorPolicyName = "ExamCreatorPolicy";

    private readonly IExamRepository _examRepository;

    public ExamController(ILogger<ExamController> logger, IMapper mapper, IExamRepository examRepository) : base(logger,
        mapper)
    {
        _examRepository = examRepository;
    }

    [HttpPost]
    [Route($"{{{ExamIdParameterName}}}/UploadTaskPdf")]
    [Authorize(ExamCreatorPolicyName)]
    public async Task<IActionResult> UploadTaskPdf(Guid examId, IFormFile taskPdfFormFile)
    {
        await using var memoryStream = new MemoryStream();
        await taskPdfFormFile.CopyToAsync(memoryStream);

        await _examRepository.SetTaskPdfFile(examId, taskPdfFormFile.FileName, GetUserId()!.Value,
            memoryStream.ToArray());

        return Ok();
    }

    [HttpPost]
    [Route($"{{{ExamIdParameterName}}}/Build")]
    [Authorize(ExamCreatorPolicyName)]
    public async Task<IActionResult> Build(Guid examId, [FromBody] BuildExamModel buildExamModel)
    {
        await _examRepository.Build(examId, buildExamModel.Count, GetUserId()!.Value);

        return Ok();
    }

    [HttpPost]
    [Route($"{{{ExamIdParameterName}}}/Match")]
    [Authorize(ExamCreatorPolicyName)]
    public async Task<IActionResult> Match(Guid examId, IFormFile submissionPdfFormFile)
    {
        await using var memoryStream = new MemoryStream();
        await submissionPdfFormFile.CopyToAsync(memoryStream);

        await _examRepository.Match(examId, memoryStream.ToArray(), GetUserId()!.Value);

        return Ok();
    }

    [HttpGet]
    [Route($"{{{ExamIdParameterName}}}/UnmatchedPages")]
    [Authorize(ExamCreatorPolicyName)]
    public UnmatchedPagesReadModel GetUnmatchedPages(Guid examId)
    {
        var unmatchedBookletPages = _examRepository.GetUnmatchedBookletPages(examId);
        var unmatchedBookletPagesMapped = Mapper.Map<IEnumerable<UnmatchedBookletPageReadModel>>(unmatchedBookletPages);

        var unmatchedSubmissionPages = _examRepository.GetUnmatchedSubmissionPages(examId);
        var unmatchedSubmissionPagesMapped =
            Mapper.Map<IEnumerable<UnmatchedSubmissionPageReadModel>>(unmatchedSubmissionPages);

        return new UnmatchedPagesReadModel
        {
            UnmatchedBookletPages = unmatchedBookletPagesMapped,
            UnmatchedSubmissionPages = unmatchedSubmissionPagesMapped
        };
    }

    [HttpPost]
    [Route($"{{{ExamIdParameterName}}}/MatchPages")]
    [Authorize(ExamCreatorPolicyName)]
    public async Task<IActionResult> MatchManually(Guid examId, ManualMatchesModel manualMatchesModel)
    {
        foreach (var manualMatchModel in manualMatchesModel.Matches)
        {
            await _examRepository.MatchManually(examId, manualMatchModel.BookletPageId,
                manualMatchModel.SubmissionPageId);
        }

        return Ok();
    }
    
    // TODO: rebuild if exam has already been built
}