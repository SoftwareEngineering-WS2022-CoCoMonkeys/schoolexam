using SchoolExam.Application.Authentication;
using SchoolExam.Application.Repository;
using SchoolExam.Application.Services;
using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.UserAggregate;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Infrastructure.Specifications;

namespace SchoolExam.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly ISchoolExamRepository _context;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(ISchoolExamRepository context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public User? GetById(Guid id)
    {
        return _context.Find(new UserByUserIdSpecification(id));
    }
    public User? GetByUsername(string username)
    {
        return _context.Find(new UserByUserNameSpecification(username));
    }

    public async Task Create(string username, string password, Role role, Guid? personId)
    {
        var userId = Guid.NewGuid();
        
        var user = new User(userId, username,_passwordHasher.HashPassword(password), role, personId);

        _context.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task Update( string username, string password, Role role, Guid? personId)
    {
        var user = EnsureUserExists(new UserByUserNameSpecification(username));
 
        user.Username = username;
        user.Password = _passwordHasher.HashPassword(password);
        user.Role = role;
        user.PersonId = personId;
        await _context.SaveChangesAsync();
    }

    public async Task Delete(String username)
    {
        var user = EnsureUserExists(new UserByUserNameSpecification(username));
        _context.Remove(user);
        await _context.SaveChangesAsync();
    }
    
    private User EnsureUserExists(EntitySpecification<User> spec)
    {
        var user = _context.Find(spec);
        if (user == null)
        {
            throw new ArgumentException("User does not exist");
        }

        return user;
    }
}

