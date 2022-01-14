namespace SchoolExam.Web.Authorization;

public class CourseStudentAuthorizationRequirement : IEntityAuthorizationRequirement
{
    public string ParameterName => RouteParameterNames.CourseIdParameterName;
}