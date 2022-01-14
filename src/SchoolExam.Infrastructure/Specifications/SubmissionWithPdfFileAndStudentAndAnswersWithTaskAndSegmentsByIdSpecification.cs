using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class
    SubmissionWithPdfFileAndStudentAndAnswersWithTaskAndSegmentsByIdSpecification : EntityByIdSpecification<Submission,
        Guid>
{
    public SubmissionWithPdfFileAndStudentAndAnswersWithTaskAndSegmentsByIdSpecification(Guid submissionId) :
        base(submissionId)
    {
        AddInclude(x => x.PdfFile);
        AddInclude(x => x.Student);
        AddInclude(x => x.Answers);
        AddInclude($"{nameof(Submission.Answers)}.{nameof(Answer.Task)}");
        AddInclude($"{nameof(Submission.Answers)}.{nameof(Answer.Segments)}");
    }
}