using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class ExamWithGradingTableById : EntityByIdSpecification<Exam, Guid>
{
    public ExamWithGradingTableById(Guid id) : base(id)
    {
        AddInclude(x => x.GradingTable);
    }
}