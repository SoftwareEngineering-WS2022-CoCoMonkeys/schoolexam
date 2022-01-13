using System;
using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.IntegrationTests.Util.Specifications;

public class ExamByCourseSpecification : EntitySpecification<Exam>
{
    public ExamByCourseSpecification(Guid courseId) : base(x => x.CourseId.Equals(courseId))
    {
    }
}