namespace SchoolExam.Web.Authorization;

public class CourseTeacherAuthorizationRequirement : IRouteParameterEntityAuthorizationRequirement
{
    public string ParameterName => RouteParameterNames.CourseIdParameterName;
}