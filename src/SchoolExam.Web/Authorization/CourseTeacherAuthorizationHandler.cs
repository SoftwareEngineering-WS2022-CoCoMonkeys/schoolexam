using SchoolExam.Application.Services;

namespace SchoolExam.Web.Authorization;

public class CourseTeacherAuthorizationHandler : EntityAuthorizationHandler<CourseTeacherAuthorizationRequirement>
{
    private readonly ICourseService _courseService;
    
    public CourseTeacherAuthorizationHandler(ICourseService courseService)
    {
        _courseService = courseService;
    }
    
    protected override bool IsAuthorized(Guid personId, Guid entityId)
    {
        var course = _courseService.GetById(entityId);
        if (course != null)
        {
            var teacherIds = course.Teachers.Select(x => x.TeacherId).ToHashSet();
            if (!teacherIds.Contains(personId))
            {
                return false;
            }
        }

        return true;
    }
}