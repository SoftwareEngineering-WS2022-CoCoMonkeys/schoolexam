using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class ExamWithBookletsWithPdfFileByIdSpecification : EntityByIdSpecification<Exam, Guid>
{
    public ExamWithBookletsWithPdfFileByIdSpecification(Guid id) : base(id)
    {
        AddInclude(x => x.Booklets);
        AddInclude($"{nameof(Exam.Booklets)}.{nameof(Booklet.PdfFile)}");
    }
}