namespace SchoolExam.Web.Authorization;

public class CourseTeacherAuthorizationRequirement : IEntityAuthorizationRequirement
{
    public string ParameterName => RouteParameterNames.CourseIdParameterName;
}