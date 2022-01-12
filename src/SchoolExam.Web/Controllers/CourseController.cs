using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolExam.Application.Services;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Web.Authorization;
using SchoolExam.Web.Models.Course;

namespace SchoolExam.Web.Controllers;

public class CourseController : ApiController<CourseController>
{
    private readonly ICourseService _courseService;
    
    public CourseController(ILogger<CourseController> logger, IMapper mapper, ICourseService courseService) :
        base(logger, mapper)
    {
        _courseService = courseService;
    }

    [HttpPost]
    [Route("Create")]
    [Authorize(Roles = Role.TeacherName)]
    public async Task<IActionResult> Create([FromBody] CourseWriteModel courseWriteModel)
    {
        await _courseService.Create(GetPersonId()!.Value, courseWriteModel.Name, courseWriteModel.Description,
            courseWriteModel.Topic);
        return Ok();
    }
    
    [HttpPut]
    [Route($"{{{RouteParameterNames.CourseIdParameterName}}}/Update")]
    [Authorize(PolicyNames.CourseTeacherPolicyName)]
    public async Task<IActionResult> Update(Guid courseId, [FromBody] CourseWriteModel courseWriteModel)
    {
        await _courseService.Update(courseId, courseWriteModel.Name, courseWriteModel.Description,
            courseWriteModel.Topic);
        return Ok();
    }

    [HttpDelete]
    [Route($"{{{RouteParameterNames.CourseIdParameterName}}}/Delete")]
    [Authorize(PolicyNames.CourseTeacherPolicyName)]
    public async Task<IActionResult> Delete(Guid courseId)
    {
        await _courseService.Delete(courseId);
        return Ok();
    }

    // TODO: add/remove teachers from/to course

    [HttpGet]
    [Route($"{{{RouteParameterNames.CourseIdParameterName}}}/TeacherView")]
    [Authorize(PolicyNames.CourseTeacherPolicyName)]
    public CourseReadModelTeacher? GetByIdTeacherView(Guid courseId)
    {
        var course = _courseService.GetById(courseId);
        return Mapper.Map<CourseReadModelTeacher>(course);
    }
    
    [HttpGet]
    [Route($"{{{RouteParameterNames.CourseIdParameterName}}}/StudentView")]
    [Authorize(PolicyNames.CourseStudentPolicyName)]
    public CourseReadModelStudent? GetByIdStudentView(Guid courseId)
    {
        var course = _courseService.GetById(courseId);
        return Mapper.Map<CourseReadModelStudent>(course);
    }

    [HttpGet]
    [Route("ByTeacher")]
    [Authorize(Roles = Role.TeacherName)]
    public IEnumerable<CourseReadModelTeacher> GetByTeacher()
    {
        var courses = _courseService.GetByTeacher(GetPersonId()!.Value);
        return Mapper.Map<IEnumerable<CourseReadModelTeacher>>(courses);
    }
}