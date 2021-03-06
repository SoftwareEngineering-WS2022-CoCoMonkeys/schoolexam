using SchoolExam.Application.Services;

namespace SchoolExam.Web.Authorization;

public class
    CourseTeacherAuthorizationHandler : RouteParameterEntityAuthorizationHandler<CourseTeacherAuthorizationRequirement>
{
    private readonly ICourseService _courseService;

    public CourseTeacherAuthorizationHandler(ICourseService courseService)
    {
        _courseService = courseService;
    }

    protected override Task<bool> IsAuthorized(Guid personId, string role, Guid entityId)
    {
        var course = _courseService.GetById(entityId);
        if (course != null)
        {
            var teacherIds = course.Teachers.Select(x => x.TeacherId).ToHashSet();
            if (!teacherIds.Contains(personId))
            {
                return Task.FromResult(false);
            }
        }

        return Task.FromResult(true);
    }
}