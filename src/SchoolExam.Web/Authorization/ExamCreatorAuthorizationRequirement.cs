namespace SchoolExam.Web.Authorization;

public class ExamCreatorAuthorizationRequirement : IEntityAuthorizationRequirement
{
    public string ParameterName => RouteParameterNames.ExamIdParameterName;
}