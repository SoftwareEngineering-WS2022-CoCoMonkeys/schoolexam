using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.UserAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class UserByUserIdSpecification : EntityByIdSpecification<User, Guid>
{
    public UserByUserIdSpecification(Guid id) : base(id)
    {
    }
}