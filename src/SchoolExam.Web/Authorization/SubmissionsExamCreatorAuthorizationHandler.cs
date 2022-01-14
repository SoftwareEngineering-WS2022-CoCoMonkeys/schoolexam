using SchoolExam.Application.Services;
using SchoolExam.Web.Models.Submission;

namespace SchoolExam.Web.Authorization;

public class
    SubmissionsExamCreatorAuthorizationHandler : RequestBodyEntityAuthorizationHandler<
        SubmissionsExamCreatorAuthorizationRequirement>
{
    private readonly ISubmissionService _submissionService;
    
    public SubmissionsExamCreatorAuthorizationHandler(ISubmissionService submissionService)
    {
        _submissionService = submissionService;
    }
    
    protected override Task<bool> IsAuthorized(Guid personId, string role, object? body)
    {
        if (body is SubmissionsByIdsModel submissionsByIdsModel)
        {
            var submissions = _submissionService.GetByIds(submissionsByIdsModel.Ids);
            return Task.FromResult(submissions.All(x => x.Booklet.Exam.CreatorId.Equals(personId)));
        }

        return Task.FromResult(false);
    }
}