using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class SubmissionWithStudentAndAnswersWithTaskAndSegmentByExamSpecification : EntitySpecification<Submission>
{
    public SubmissionWithStudentAndAnswersWithTaskAndSegmentByExamSpecification(Guid examId) : base(x => x.Booklet.ExamId.Equals(examId))
    {
        AddInclude(x => x.Booklet);
        AddInclude(x => x.Student);
        AddInclude(x => x.Answers);
        AddInclude($"{nameof(Submission.Answers)}.{nameof(Answer.Task)}");
        AddInclude($"{nameof(Submission.Answers)}.{nameof(Answer.Segments)}");
    }
}