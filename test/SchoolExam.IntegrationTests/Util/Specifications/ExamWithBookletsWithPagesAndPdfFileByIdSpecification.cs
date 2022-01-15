using System;
using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.IntegrationTests.Util.Specifications;

public class ExamWithBookletsWithPagesAndPdfFileByIdSpecification : EntityByIdSpecification<Exam>
{
    public ExamWithBookletsWithPagesAndPdfFileByIdSpecification(Guid id) : base(id)
    {
        AddInclude(x => x.Booklets);
        AddInclude($"{nameof(Exam.Booklets)}.{nameof(Booklet.Pages)}");
        AddInclude($"{nameof(Exam.Booklets)}.{nameof(Booklet.PdfFile)}");
    }
}