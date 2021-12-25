using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SchoolExam.Web.Exam;

public class ExamController : ApiController<ExamController>
{
    public const string ExamIdParameter = "examId";
    
    public ExamController(ILogger<ExamController> logger) : base(logger)
    {
    }

    [HttpDelete]
    [Route($"{ExamIdParameter}")]
    [Authorize]
    public IActionResult DeleteExam(string examId)
    {
        return Ok();
    }
}