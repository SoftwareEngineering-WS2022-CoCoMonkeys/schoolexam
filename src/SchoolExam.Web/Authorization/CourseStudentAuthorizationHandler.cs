using SchoolExam.Application.Services;

namespace SchoolExam.Web.Authorization;

public class
    CourseStudentAuthorizationHandler : RouteParameterEntityAuthorizationHandler<CourseStudentAuthorizationRequirement>
{
    private readonly ICourseService _courseService;

    public CourseStudentAuthorizationHandler(ICourseService courseService)
    {
        _courseService = courseService;
    }

    protected override Task<bool> IsAuthorized(Guid personId, string role, Guid entityId)
    {
        var course = _courseService.GetById(entityId);
        if (course != null)
        {
            var studentIds = course.Students.Select(x => x.StudentId).ToHashSet();
            if (!studentIds.Contains(personId))
            {
                return Task.FromResult(false);
            }
        }

        return Task.FromResult(true);
    }
}