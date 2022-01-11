using SchoolExam.Application.Services;

namespace SchoolExam.Web.Authorization;

public class CourseStudentAuthorizationHandler : EntityAuthorizationHandler<CourseStudentAuthorizationRequirement>
{
    private readonly ICourseService _courseService;
    
    public CourseStudentAuthorizationHandler(ICourseService courseService)
    {
        _courseService = courseService;
    }
    
    protected override bool IsAuthorized(Guid personId, Guid entityId)
    {
        var course = _courseService.GetById(entityId);
        if (course != null)
        {
            var studentIds = course.Students.Select(x => x.StudentId).ToHashSet();
            if (!studentIds.Contains(personId))
            {
                return false;
            }
        }

        return true;
    }
}