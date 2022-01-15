using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.PersonAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class StudentByIdSpecification: EntityByIdSpecification<Student, Guid>
{
    public StudentByIdSpecification(Guid id) : base(id)
    {
    }
}