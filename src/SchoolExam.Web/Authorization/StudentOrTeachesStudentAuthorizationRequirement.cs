namespace SchoolExam.Web.Authorization;

public class StudentOrTeachesStudentAuthorizationRequirement : IRouteParameterEntityAuthorizationRequirement
{
    public string ParameterName => RouteParameterNames.StudentIdParameterName;
}