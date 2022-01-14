using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.UserAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class UserByUserNameSpecification: EntitySpecification<User>
{
    public UserByUserNameSpecification(string username) : base(x => x.Username.Equals(username))
    {
    }
}