using System;
using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.IntegrationTests.Util.Specifications;

public class ExamBookletWithPagesByExamSpecification : EntitySpecification<ExamBooklet>
{
    public ExamBookletWithPagesByExamSpecification(Guid examId) : base(x => x.ExamId.Equals(examId))
    {
        AddInclude(x => x.Pages);
    }
}