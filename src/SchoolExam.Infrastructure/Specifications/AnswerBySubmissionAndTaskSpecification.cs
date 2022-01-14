using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class AnswerBySubmissionAndTaskSpecification : EntitySpecification<Answer>
{
    public AnswerBySubmissionAndTaskSpecification(Guid submissionId, Guid taskId) : base(x =>
        x.SubmissionId.Equals(submissionId) && x.TaskId.Equals(taskId))
    {
    }
}