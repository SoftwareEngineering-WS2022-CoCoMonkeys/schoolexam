using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class ExamWithBookletsWithPagesByIdSpecification : EntityByIdSpecification<Exam>
{
    public ExamWithBookletsWithPagesByIdSpecification(Guid id) : base(id)
    {
        AddInclude(x => x.Booklets);
        AddInclude($"{nameof(Exam.Booklets)}.{nameof(Booklet.Pages)}");
    }
}