namespace SchoolExam.Web.Authorization;

public class CourseStudentAuthorizationRequirement : IRouteParameterEntityAuthorizationRequirement
{
    public string ParameterName => RouteParameterNames.CourseIdParameterName;
}