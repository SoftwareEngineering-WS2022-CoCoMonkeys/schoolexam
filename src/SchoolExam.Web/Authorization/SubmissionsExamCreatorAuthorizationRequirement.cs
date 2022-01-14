using SchoolExam.Web.Models.Submission;

namespace SchoolExam.Web.Authorization;

public class SubmissionsExamCreatorAuthorizationRequirement : IRequestBodyEntityAuthorizationRequirement
{
    public Type Type => typeof(SubmissionsByIdsModel);
}