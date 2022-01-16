using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class ExamWithTaskPdfFileAndGradingTableByIdSpecification : EntityByIdSpecification<Exam>
{
    public ExamWithTaskPdfFileAndGradingTableByIdSpecification(Guid id) : base(id)
    {
        AddInclude(x => x.TaskPdfFile);
        AddInclude(x => x.GradingTable);
    }
}