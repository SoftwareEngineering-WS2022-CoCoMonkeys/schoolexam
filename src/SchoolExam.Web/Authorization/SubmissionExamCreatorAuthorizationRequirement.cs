namespace SchoolExam.Web.Authorization;

public class SubmissionExamCreatorAuthorizationRequirement : IRouteParameterEntityAuthorizationRequirement
{
    public string ParameterName => RouteParameterNames.SubmissionIdParameterName;
}