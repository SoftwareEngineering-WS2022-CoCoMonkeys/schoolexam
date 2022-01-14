namespace SchoolExam.Web.Authorization;

public class StudentOrTeachesStudentAuthorizationRequirement : IEntityAuthorizationRequirement
{
    public string ParameterName => RouteParameterNames.StudentIdParameterName;
}