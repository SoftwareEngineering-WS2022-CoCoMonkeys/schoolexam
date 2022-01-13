using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class ExamWithTaskPdfFileAndTasksByIdSpecification : EntityByIdSpecification<Exam, Guid>
{
    public ExamWithTaskPdfFileAndTasksByIdSpecification(Guid id) : base(id)
    {
        AddInclude(x => x.TaskPdfFile);
        AddInclude(x => x.Tasks);
    }
}