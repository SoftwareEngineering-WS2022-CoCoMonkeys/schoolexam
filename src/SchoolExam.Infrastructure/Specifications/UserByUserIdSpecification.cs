using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.UserAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class UserByUserIdSpecification : EntitySpecification<User>
{
    public UserByUserIdSpecification(Guid id) : base(x => x.Id.Equals(id))
    {
        //return _context.Users.SingleOrDefault(x => x.Username.Equals(username));
    }
}