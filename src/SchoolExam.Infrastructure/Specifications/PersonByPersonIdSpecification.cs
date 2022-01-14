using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.Entities.UserAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class PersonByPersonIdSpecification: EntityByIdSpecification<Person, Guid>
{
    public PersonByPersonIdSpecification(Guid id) : base(id)
    {
    }
}