namespace SchoolExam.Web.Authorization;

public class ExamCreatorAuthorizationRequirement : IRouteParameterEntityAuthorizationRequirement
{
    public string ParameterName => RouteParameterNames.ExamIdParameterName;
}