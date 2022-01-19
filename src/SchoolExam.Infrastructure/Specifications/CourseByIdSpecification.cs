using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.CourseAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class CourseByIdSpecification : EntityByIdSpecification<Course>
{
    public CourseByIdSpecification(Guid id) : base(id)
    {
        AddInclude(x => x.Students);
        AddInclude($"{nameof(Course.Students)}.{nameof(CourseStudent.Student)}");
        AddInclude(x => x.Teachers);
    }
}