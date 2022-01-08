using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolExam.Application.Repositories;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Web.Authorization;
using SchoolExam.Web.Models.Course;

namespace SchoolExam.Web.Controllers;

public class CourseController : ApiController<CourseController>
{
    private readonly ICourseRepository _courseRepository;
    
    public CourseController(ILogger<CourseController> logger, IMapper mapper, ICourseRepository courseRepository) :
        base(logger, mapper)
    {
        _courseRepository = courseRepository;
    }

    [HttpPost]
    [Route("Create")]
    [Authorize(Roles = Role.TeacherName)]
    public async Task<IActionResult> Create([FromBody] CourseWriteModel courseWriteModel)
    {
        await _courseRepository.Create(GetPersonId()!.Value, courseWriteModel.Name, courseWriteModel.Description,
            courseWriteModel.Subject);
        return Ok();
    }
    
    [HttpPut]
    [Route($"{{{RouteParameterNames.CourseIdParameterName}}}/Update")]
    [Authorize(PolicyNames.CourseTeacherPolicyName)]
    public async Task<IActionResult> Update(Guid courseId, [FromBody] CourseWriteModel courseWriteModel)
    {
        await _courseRepository.Update(courseId, courseWriteModel.Name, courseWriteModel.Description,
            courseWriteModel.Subject);
        return Ok();
    }

    [HttpDelete]
    [Route($"{{{RouteParameterNames.CourseIdParameterName}}}/Delete")]
    [Authorize(PolicyNames.CourseTeacherPolicyName)]
    public async Task<IActionResult> Delete(Guid courseId)
    {
        await _courseRepository.Delete(courseId);
        return Ok();
    }

    // TODO: add/remove teachers from/to
    
    [HttpDelete]

    [HttpGet]
    [Route($"{{{RouteParameterNames.CourseIdParameterName}}}/TeacherView")]
    [Authorize(PolicyNames.CourseTeacherPolicyName)]
    public CourseReadModelTeacher? GetByIdTeacherView(Guid courseId)
    {
        var course = _courseRepository.GetById(courseId);
        return Mapper.Map<CourseReadModelTeacher>(course);
    }
    
    [HttpGet]
    [Route($"{{{RouteParameterNames.CourseIdParameterName}}}/StudentView")]
    [Authorize(PolicyNames.CourseStudentPolicyName)]
    public CourseReadModelTeacher? GetByIdStudentView(Guid courseId)
    {
        var course = _courseRepository.GetById(courseId);
        return Mapper.Map<CourseReadModelTeacher>(course);
    }

    [HttpGet]
    [Route("ByTeacher")]
    [Authorize(Roles = Role.TeacherName)]
    public IEnumerable<CourseReadModelTeacher> GetByTeacher()
    {
        var courses = _courseRepository.GetByTeacher(GetPersonId()!.Value);
        return Mapper.Map<IEnumerable<CourseReadModelTeacher>>(courses);
    }
}