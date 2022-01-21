using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class BookletWithSubmissionWithStudentWithRemarkPdfAndPdfFileByExamSpecification  : EntitySpecification<Booklet>
{
    public BookletWithSubmissionWithStudentWithRemarkPdfAndPdfFileByExamSpecification(Guid examId) : base(x =>
        x.ExamId.Equals(examId) && x.Submission != null)
    {
        AddInclude(x => x.Submission);
        AddInclude($"{nameof(Booklet.Submission)}.{nameof(Submission.Student)}");
        AddInclude($"{nameof(Booklet.Submission)}.{nameof(Submission.RemarkPdfFile)}");
        AddInclude($"{nameof(Booklet.Submission)}.{nameof(Submission.PdfFile)}");
    }
}