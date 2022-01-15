using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class ExamTaskByExamSpecification : EntitySpecification<ExamTask>
{
    public ExamTaskByExamSpecification(Guid examId) : base(x => x.ExamId.Equals(examId)) 
    {
    }
}