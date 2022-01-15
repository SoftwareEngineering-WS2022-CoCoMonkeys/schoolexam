using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.CourseAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class CourseByIdSpecification : EntityByIdSpecification<Course>
{
    public CourseByIdSpecification(Guid id) : base(id)
    {
        AddInclude(x => x.Students);
        AddInclude(x => x.Teachers);
    }
}