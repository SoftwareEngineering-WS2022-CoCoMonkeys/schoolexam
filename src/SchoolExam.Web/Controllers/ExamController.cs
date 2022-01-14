using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolExam.Application.Services;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Web.Authorization;
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
    public async Task<IActionResult> Create([FromBody] ExamWriteModel examWriteModel)
    {
        await _examService.Create(examWriteModel.Title, examWriteModel.Description, examWriteModel.Date,
            GetPersonId()!.Value, examWriteModel.Topic);
        return Ok();
    }

    [HttpPut]
    [Route($"{{{RouteParameterNames.ExamIdParameterName}}}/Update")]
    [Authorize(PolicyNames.ExamCreatorPolicyName)]
    public async Task<IActionResult> Update(Guid examId, [FromBody] ExamWriteModel examWriteModel)
    {
        await _examService.Update(examId, examWriteModel.Title, examWriteModel.Description, examWriteModel.Date);
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
    [Route($"{{{RouteParameterNames.ExamIdParameterName}}}/UploadTaskPdf")]
    [Authorize(PolicyNames.ExamCreatorPolicyName)]
    public async Task<IActionResult> UploadTaskPdf(Guid examId, [FromBody] UploadTaskPdfModel uploadTaskPdfModel)
    {
        var pdf = Convert.FromBase64String(uploadTaskPdfModel.TaskPdf);

        await _examService.SetTaskPdfFile(examId, GetUserId()!.Value, pdf);
        var tasks = Mapper.Map<IEnumerable<ExamTaskInfo>>(uploadTaskPdfModel.Tasks).ToArray();
        await _examService.FindTasks(examId, GetUserId()!.Value, tasks);

        return Ok();
    }

    [HttpPost]
    [Route($"{{{RouteParameterNames.ExamIdParameterName}}}/Build")]
    [Authorize(PolicyNames.ExamCreatorPolicyName)]
    public async Task<BuildResultModel> Build(Guid examId)
    {
        var count = await _examService.Build(examId, GetUserId()!.Value);
        var pdf = _examService.GetConcatenatedBookletPdfFile(examId);
        return new BuildResultModel {Count = count, PdfFile = Convert.ToBase64String(pdf)};
    }

    [HttpPost]
    [Route($"{{{RouteParameterNames.ExamIdParameterName}}}/SubmitAndMatch")]
    [Authorize(PolicyNames.ExamCreatorPolicyName)]
    public async Task<IActionResult> SubmitAndMatch(Guid examId, [FromBody] SubmitAndMatchModel submitAndMatchModel)
    {
        var pdf = Convert.FromBase64String(submitAndMatchModel.Pdf);

        await _examService.Match(examId, pdf, GetUserId()!.Value);

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
}