using SchoolExam.Application.Services;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Web.Authorization;

public class StudentOrTeachesStudentAuthorizationHandler : RouteParameterEntityAuthorizationHandler<
    StudentOrTeachesStudentAuthorizationRequirement>
{
    private readonly ICourseService _courseService;

    public StudentOrTeachesStudentAuthorizationHandler(ICourseService courseService)
    {
        _courseService = courseService;
    }

    protected override Task<bool> IsAuthorized(Guid personId, string role, Guid entityId)
    {
        if (Role.Student.Name.Equals(role))
        {
            return Task.FromResult(personId.Equals(entityId));
        }

        if (Role.Teacher.Name.Equals(role))
        {
            var courses = _courseService.GetByTeacher(personId);
            var studentIds = courses.SelectMany(x => x.Students).Select(x => x.StudentId).ToHashSet();
            return Task.FromResult(studentIds.Contains(entityId));
        }

        return Task.FromResult(false);
    }
}