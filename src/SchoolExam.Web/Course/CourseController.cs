using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolExam.Application.Repositories;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Web.Course;

public class CourseController : ApiController<CourseController>
{
    public const string CourseIdParameterName = "courseId";
    public const string CourseTeacherPolicyName = "CourseTeacherPolicy";
    public const string CourseStudentPolicyName = "CourseStudentPolicy";
    
    private readonly IMapper _mapper;
    private readonly ICourseRepository _courseRepository;
    
    public CourseController(ILogger<CourseController> logger, IMapper mapper, ICourseRepository courseRepository) :
        base(logger)
    {
        _mapper = mapper;
        _courseRepository = courseRepository;
    }

    [HttpGet]
    [Route($"{{{CourseIdParameterName}}}/TeacherView")]
    [Authorize(CourseTeacherPolicyName)]
    public CourseReadModelTeacher? GetByIdTeacherView(Guid courseId)
    {
        var course = _courseRepository.GetById(courseId);
        return _mapper.Map<CourseReadModelTeacher>(course);
    }
    
    [HttpGet]
    [Route($"{{{CourseIdParameterName}}}/StudentView")]
    [Authorize(CourseStudentPolicyName)]
    public CourseReadModelTeacher? GetByIdStudentView(Guid courseId)
    {
        var course = _courseRepository.GetById(courseId);
        return _mapper.Map<CourseReadModelTeacher>(course);
    }

    [HttpGet]
    [Route("ByTeacher/{teacherId}")]
    [Authorize(Roles = Role.TeacherName)]
    public IEnumerable<CourseReadModelTeacher> GetByTeacher(Guid teacherId)
    {
        var courses = _courseRepository.GetByTeacher(teacherId);
        return _mapper.Map<IEnumerable<CourseReadModelTeacher>>(courses);
    }
}