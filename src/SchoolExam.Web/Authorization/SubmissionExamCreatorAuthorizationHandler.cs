using SchoolExam.Application.Services;

namespace SchoolExam.Web.Authorization;

public class
    SubmissionExamCreatorAuthorizationHandler : EntityAuthorizationHandler<
        SubmissionExamCreatorAuthorizationRequirement>
{
    private readonly ISubmissionService _submissionService;

    public SubmissionExamCreatorAuthorizationHandler(ISubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    protected override bool IsAuthorized(Guid personId, string role, Guid entityId)
    {
        var submission = _submissionService.GetById(entityId);
        if (submission != null)
        {
            var creatorId = submission.Booklet.Exam.CreatorId;
            if (!creatorId.Equals(personId))
            {
                return false;
            }
        }

        return true;
    }
}