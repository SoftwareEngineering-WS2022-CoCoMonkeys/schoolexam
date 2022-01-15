using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class ExamWithTasksByIdSpecification : EntityByIdSpecification<Exam>
{
    public ExamWithTasksByIdSpecification(Guid id) : base(id)
    {
        AddInclude(x => x.Tasks);
    }
}