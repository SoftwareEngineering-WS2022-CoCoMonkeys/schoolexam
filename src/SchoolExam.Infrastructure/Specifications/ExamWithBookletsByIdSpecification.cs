using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class ExamWithBookletsByIdSpecification : EntityByIdSpecification<Exam>
{
    public ExamWithBookletsByIdSpecification(Guid id) : base(id)
    {
        AddInclude(x => x.Booklets);
    }
}