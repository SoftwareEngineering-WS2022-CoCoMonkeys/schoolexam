using SchoolExam.Application.DataContext;
using SchoolExam.Application.Repositories;
using SchoolExam.Domain.Entities.UserAggregate;

namespace SchoolExam.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ISchoolExamDataContext _context;

    public UserRepository(ISchoolExamDataContext context)
    {
        _context = context;
    }

    public User? GetByUsername(string username)
    {
        return _context.Users.SingleOrDefault(x => x.Username.Equals(username));
    }
}