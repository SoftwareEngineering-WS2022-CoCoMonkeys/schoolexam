using SchoolExam.Application.Repository;
using SchoolExam.Application.Services;
using SchoolExam.Domain.Entities.UserAggregate;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Infrastructure.Specifications;

namespace SchoolExam.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly ISchoolExamRepository _context;

    public UserService(ISchoolExamRepository context)
    {
        _context = context;
    }

    public User? GetByUsername(string username)
    {
        return _context.Find(new UserByUserIdSpecification(username));
    }

    public async Task Update(Guid userId, string username, string password, Role role, Guid personId)
    {
        var user = _context.Find(new UserByUserNameSpecification(username));
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