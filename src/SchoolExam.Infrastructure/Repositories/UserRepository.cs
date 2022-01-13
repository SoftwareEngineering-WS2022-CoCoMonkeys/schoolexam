using SchoolExam.Application.DataContext;
using SchoolExam.Application.Repositories;
using SchoolExam.Domain.Entities.UserAggregate;
using SchoolExam.Domain.ValueObjects;

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
    
    public async Task Create(string username, string password, Role role,  Guid personId)
    {
        var userId = Guid.NewGuid();
        var user = new User(userId, username, password, role, personId);
        
        _context.Add(user);
        
        await _context.SaveChangesAsync();
    }

    public async Task Update(Guid userId, string username, string password, Role role, Guid personId)
    {
        var user = _context.Users.SingleOrDefault(x  => x.Id.Equals(userId));
        if (user == null)
        {
            throw new ArgumentException("User does not exist");
        }

        user.Username = username;
        user.Password = password;
        user.Role = role;
        
        
        
        await _context.SaveChangesAsync();
    }
}