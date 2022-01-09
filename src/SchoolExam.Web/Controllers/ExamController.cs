using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolExam.Application.Repositories;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Web.Authorization;
using SchoolExam.Web.Models.Exam;

namespace SchoolExam.Web.Controllers;

public class ExamController : ApiController<ExamController>
{
    private readonly IExamRepository _examRepository;

    public ExamController(ILogger<ExamController> logger, IMapper mapper, IExamRepository examRepository) : base(logger,
        mapper)
    {
        _examRepository = examRepository;
    }

    [HttpGet]
    [Route("ByTeacher")]
    [Authorize(Roles = Role.TeacherName)]
    public IEnumerable<ExamReadModelTeacher> GetByTeacher()
    {
        var exams = _examRepository.GetByTeacher(GetPersonId()!.Value);
        return Mapper.Map<IEnumerable<ExamReadModelTeacher>>(exams);
    }

    [HttpPost]
    [Route($"Create/{{{RouteParameterNames.CourseIdParameterName}}}")]
    [Authorize(PolicyNames.CourseTeacherPolicyName)]
    public async Task<IActionResult> Create(Guid courseId, [FromBody] ExamWriteModel examWriteModel)
    {
        await _examRepository.Create(examWriteModel.Title, examWriteModel.Description, examWriteModel.Date, courseId,
            GetPersonId()!.Value);
        return Ok();
    }

    [HttpPut]
    [Route($"{{{RouteParameterNames.ExamIdParameterName}}}/Update")]
    [Authorize(PolicyNames.ExamCreatorPolicyName)]
    public async Task<IActionResult> Update(Guid examId, [FromBody] ExamWriteModel examWriteModel)
    {
        await _examRepository.Update(examId, examWriteModel.Title, examWriteModel.Description, examWriteModel.Date);
        return Ok();
    }

    [HttpDelete]
    [Route($"{{{RouteParameterNames.ExamIdParameterName}}}/Delete")]
    [Authorize(PolicyNames.ExamCreatorPolicyName)]
    public async Task<IActionResult> Delete(Guid examId)
    {
        await _examRepository.Delete(examId);
        return Ok();
    }

    [HttpPost]
    [Route($"{{{RouteParameterNames.ExamIdParameterName}}}/UploadTaskPdf")]
    [Authorize(PolicyNames.ExamCreatorPolicyName)]
    public async Task<IActionResult> UploadTaskPdf(Guid examId, [FromBody] UploadTaskPdfModel uploadTaskPdfModel)
    {
        var pdf = Convert.FromBase64String(uploadTaskPdfModel.TaskPdf);

        await _examRepository.SetTaskPdfFile(examId, GetUserId()!.Value, pdf);
        var tasks = Mapper.Map<IEnumerable<ExamTaskInfo>>(uploadTaskPdfModel.Tasks).ToArray();
        await _examRepository.FindTasks(examId, GetUserId()!.Value, tasks);

        return Ok();
    }

    [HttpPost]
    [Route($"{{{RouteParameterNames.ExamIdParameterName}}}/Build")]
    [Authorize(PolicyNames.ExamCreatorPolicyName)]
    public async Task<IActionResult> Build(Guid examId, [FromBody] BuildExamModel buildExamModel)
    {
        await _examRepository.Build(examId, buildExamModel.Count, GetUserId()!.Value);

        return Ok();
    }

    [HttpPost]
    [Route($"{{{RouteParameterNames.ExamIdParameterName}}}/Clean")]
    [Authorize(PolicyNames.ExamCreatorPolicyName)]
    public async Task<IActionResult> Clean(Guid examId)
    {
        await _examRepository.Clean(examId);

        return Ok();
    }

    [HttpPost]
    [Route($"{{{RouteParameterNames.ExamIdParameterName}}}/Rebuild")]
    [Authorize(PolicyNames.ExamCreatorPolicyName)]
    public async Task<IActionResult> Rebuild(Guid examId, [FromBody] BuildExamModel buildExamModel)
    {
        await _examRepository.Clean(examId);
        await _examRepository.Build(examId, buildExamModel.Count, GetUserId()!.Value);

        return Ok();
    }

    [HttpPost]
    [Route($"{{{RouteParameterNames.ExamIdParameterName}}}/Match")]
    [Authorize(PolicyNames.ExamCreatorPolicyName)]
    public async Task<IActionResult> Match(Guid examId, IFormFile submissionPdfFormFile)
    {
        await using var memoryStream = new MemoryStream();
        await submissionPdfFormFile.CopyToAsync(memoryStream);

        await _examRepository.Match(examId, memoryStream.ToArray(), GetUserId()!.Value);

        return Ok();
    }

    [HttpGet]
    [Route($"{{{RouteParameterNames.ExamIdParameterName}}}/UnmatchedPages")]
    [Authorize(PolicyNames.ExamCreatorPolicyName)]
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
    [Route($"{{{RouteParameterNames.ExamIdParameterName}}}/MatchPages")]
    [Authorize(PolicyNames.ExamCreatorPolicyName)]
    public async Task<IActionResult> MatchManually(Guid examId, [FromBody] ManualMatchesModel manualMatchesModel)
    {
        foreach (var manualMatchModel in manualMatchesModel.Matches)
        {
            await _examRepository.MatchManually(examId, manualMatchModel.BookletPageId,
                manualMatchModel.SubmissionPageId, GetUserId()!.Value);
        }

        return Ok();
    }
}