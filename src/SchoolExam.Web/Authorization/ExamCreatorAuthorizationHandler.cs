using SchoolExam.Application.Services;

namespace SchoolExam.Web.Authorization;

public class ExamCreatorAuthorizationHandler : EntityAuthorizationHandler<ExamCreatorAuthorizationRequirement>
{
    private readonly IExamService _examService;
    
    public ExamCreatorAuthorizationHandler(IExamService examService)
    {
        _examService = examService;
    }
    
    protected override bool IsAuthorized(Guid personId, string role, Guid entityId)
    {
        var exam = _examService.GetById(entityId);
        if (exam != null)
        {
            var creatorId = exam.CreatorId;
            if (!creatorId.Equals(personId))
            {
                return false;
            }
        }

        return true;
    }
}