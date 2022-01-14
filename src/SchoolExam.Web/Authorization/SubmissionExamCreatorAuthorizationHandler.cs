using SchoolExam.Application.Services;

namespace SchoolExam.Web.Authorization;

public class
    SubmissionExamCreatorAuthorizationHandler : RouteParameterEntityAuthorizationHandler<
        SubmissionExamCreatorAuthorizationRequirement>
{
    private readonly ISubmissionService _submissionService;

    public SubmissionExamCreatorAuthorizationHandler(ISubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    protected override Task<bool> IsAuthorized(Guid personId, string role, Guid entityId)
    {
        var submission = _submissionService.GetById(entityId);
        if (submission != null)
        {
            var creatorId = submission.Booklet.Exam.CreatorId;
            if (!creatorId.Equals(personId))
            {
                return Task.FromResult(false);
            }
        }

        return Task.FromResult(true);
    }
}