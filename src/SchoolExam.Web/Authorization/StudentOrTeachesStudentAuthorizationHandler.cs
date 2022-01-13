using SchoolExam.Application.Services;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Web.Authorization;

public class StudentOrTeachesStudentAuthorizationHandler : EntityAuthorizationHandler<
    StudentOrTeachesStudentAuthorizationRequirement>
{
    private readonly ICourseService _courseService;
    
    public StudentOrTeachesStudentAuthorizationHandler(ICourseService courseService)
    {
        _courseService = courseService;
    }
    protected override bool IsAuthorized(Guid personId, string role, Guid entityId)
    {
        if (Role.Student.Name.Equals(role))
        {
            return personId.Equals(entityId);
        }
        if (Role.Teacher.Name.Equals(role))
        {
            var courses = _courseService.GetByTeacher(personId);
            var studentIds = courses.SelectMany(x => x.Students).Select(x => x.StudentId).ToHashSet();
            return studentIds.Contains(entityId);
        }

        return false;
    }
}