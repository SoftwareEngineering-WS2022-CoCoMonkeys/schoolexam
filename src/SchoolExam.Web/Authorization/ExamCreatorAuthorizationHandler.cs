using SchoolExam.Application.Services;

namespace SchoolExam.Web.Authorization;

public class
    ExamCreatorAuthorizationHandler : RouteParameterEntityAuthorizationHandler<ExamCreatorAuthorizationRequirement>
{
    private readonly IExamManagementService _examManagementService;

    public ExamCreatorAuthorizationHandler(IExamManagementService examManagementService)
    {
        _examManagementService = examManagementService;
    }

    protected override Task<bool> IsAuthorized(Guid personId, string role, Guid entityId)
    {
        var exam = _examManagementService.GetById(entityId);
        if (exam != null)
        {
            var creatorId = exam.CreatorId;
            if (!creatorId.Equals(personId))
            {
                return Task.FromResult(false);
            }
        }

        return Task.FromResult(true);
    }
}