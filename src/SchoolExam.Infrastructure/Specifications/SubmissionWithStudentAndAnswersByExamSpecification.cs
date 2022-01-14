using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class SubmissionWithPdfFileAndStudentAndAnswersByExamSpecification : EntitySpecification<Submission>
{
    public SubmissionWithPdfFileAndStudentAndAnswersByExamSpecification(Guid examId) : base(x =>
        x.Booklet.ExamId.Equals(examId))
    {
        AddInclude(x => x.PdfFile);
        AddInclude(x => x.Booklet);
        AddInclude(x => x.Student);
        AddInclude(x => x.Answers);
    }
}