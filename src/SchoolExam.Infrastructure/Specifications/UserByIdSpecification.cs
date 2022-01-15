using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.UserAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class UserByIdSpecification : EntityByIdSpecification<User>
{
    public UserByIdSpecification(Guid id) : base(id)
    {
    }
}