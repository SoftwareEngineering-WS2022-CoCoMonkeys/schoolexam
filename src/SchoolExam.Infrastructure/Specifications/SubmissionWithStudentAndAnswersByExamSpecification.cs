using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class SubmissionWithStudentAndAnswersByExamSpecification : EntitySpecification<Submission>
{
    public SubmissionWithStudentAndAnswersByExamSpecification(Guid examId) : base(x => x.Booklet.ExamId.Equals(examId))
    {
        AddInclude(x => x.Booklet);
        AddInclude(x => x.Student);
        AddInclude(x => x.Answers);
    }
}