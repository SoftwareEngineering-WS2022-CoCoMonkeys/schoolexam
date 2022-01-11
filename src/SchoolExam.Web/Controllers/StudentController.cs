using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolExam.Application.Services;
using SchoolExam.Web.Authorization;

namespace SchoolExam.Web.Controllers;

public class StudentController : ApiController<StudentController>
{
    private readonly IStudentService _studentService;
    
    public StudentController(ILogger<StudentController> logger, IMapper mapper, IStudentService studentService) :
        base(logger, mapper)
    {
        _studentService = studentService;
    }

    [HttpGet]
    [Route($"{{{RouteParameterNames.StudentIdParameterName}}}/QrCode")]
    [Authorize(PolicyNames.StudentOrTeachesStudentPolicyName)]
    public IActionResult GetQrCode(Guid studentId)
    {
        var qrCode = _studentService.GetQrCodeByStudentId(studentId);
        return File(qrCode, "application/png");
    }
}