using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class ExamWithTaskPdfFileByIdSpecification : EntityByIdSpecification<Exam>
{
    public ExamWithTaskPdfFileByIdSpecification(Guid id) : base(id)
    {
        AddInclude(x => x.TaskPdfFile);
    }
}