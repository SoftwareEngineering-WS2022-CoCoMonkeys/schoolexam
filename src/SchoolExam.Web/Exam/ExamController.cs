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

        // maximum file size of 50MB
        // TODO: make maximum file size configurable
        if (memoryStream.Length > 52428800)
        {
            return this.BadRequest();
        }

        await _examRepository.SetTaskPdfFile(examId, taskPdfFormFile.FileName, GetUserId()!.Value,
            memoryStream.ToArray());

        return Ok();
    }
}