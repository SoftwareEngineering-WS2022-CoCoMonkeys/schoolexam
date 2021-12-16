using SchoolExam.Core.UserManagement.UserAggregate;
using SchoolExam.Infrastructure.DataContext;

namespace SchoolExam.Infrastructure.Repository;

public class UserRepository : IUserRepository
{
    private readonly SchoolExamDataContext _context;

    public UserRepository(SchoolExamDataContext context)
    {
        _context = context;
    }

    public User? GetByUsername(string username)
    {
        return _context.Users.SingleOrDefault(x => x.Username.Equals(username));
    }
}