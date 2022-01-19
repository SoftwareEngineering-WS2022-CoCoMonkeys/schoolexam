using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class ExamWithGradingTableAndTasksById : EntityByIdSpecification<Exam>
{
    public ExamWithGradingTableAndTasksById(Guid id) : base(id)
    {
        AddInclude(x => x.GradingTable);
        AddInclude(x => x.Tasks);
    }
}