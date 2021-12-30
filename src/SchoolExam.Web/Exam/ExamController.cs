using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolExam.Application.Repositories;

namespace SchoolExam.Web.Exam;

public class ExamController : ApiController<ExamController>
{
    public const string ExamIdParameterName = "examId";
    public const string ExamCreatorPolicyName = "ExamCreatorPolicy";

    private readonly IExamRepository _examRepository;

    public ExamController(ILogger<ExamController> logger, IExamRepository examRepository) : base(logger)
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

    // TODO: HttpGet Conflicts (two (or more) different submission pages (based on content) mapped to same booklet page)
    // TODO: HttpGet Unmatched (page that has no corresponding submission page)
    // TODO: HttpPut Match manually
}