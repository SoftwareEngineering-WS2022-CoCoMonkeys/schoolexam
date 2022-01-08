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