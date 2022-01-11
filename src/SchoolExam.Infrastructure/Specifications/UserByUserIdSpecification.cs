using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.UserAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class UserByUserIdSpecification : EntitySpecification<User>
{
    public UserByUserIdSpecification(string username) : base(x => x.Username.Equals(username))
    {
    }
}