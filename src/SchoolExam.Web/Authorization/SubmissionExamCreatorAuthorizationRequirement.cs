namespace SchoolExam.Web.Authorization;

public class SubmissionExamCreatorAuthorizationRequirement : IEntityAuthorizationRequirement
{
    public string ParameterName => RouteParameterNames.SubmissionIdParameterName;
}