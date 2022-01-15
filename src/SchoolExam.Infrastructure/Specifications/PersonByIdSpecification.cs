using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.PersonAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class PersonByIdSpecification: EntityByIdSpecification<Person>
{
    public PersonByIdSpecification(Guid id) : base(id)
    {
    }
}