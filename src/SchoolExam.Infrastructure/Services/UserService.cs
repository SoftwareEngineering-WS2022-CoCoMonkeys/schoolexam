using SchoolExam.Application.Authentication;
using SchoolExam.Application.Repository;
using SchoolExam.Application.Services;
using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.UserAggregate;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Infrastructure.Extensions;
using SchoolExam.Infrastructure.Specifications;

namespace SchoolExam.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly ISchoolExamRepository _repository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(ISchoolExamRepository repository, IPasswordHasher passwordHasher)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
    }

    public User? GetById(Guid id)
    {
        return _repository.Find(new UserByIdSpecification(id));
    }
    public User? GetByUsername(string username)
    {
        return _repository.Find(new UserByUserNameSpecification(username));
    }

    public IEnumerable<User> GetAllUsers()
    {
        return _repository.List<User>();
    }

    public async Task Create(string username, string password, Role role, Guid? personId)
    {
        var userId = Guid.NewGuid();
        
        var user = new User(userId, username,_passwordHasher.HashPassword(password), role, personId);

        _repository.Add(user);
        await _repository.SaveChangesAsync();
    }

    public async Task Update( string username, string password, Role role, Guid? personId)
    {
        var user = EnsureUserExists(new UserByUserNameSpecification(username));
 
        user.Username = username;
        user.Password = _passwordHasher.HashPassword(password);
        user.Role = role;
        user.PersonId = personId;
        await _repository.SaveChangesAsync();
    }

    public async Task Delete(String username)
    {
        var user = EnsureUserExists(new UserByUserNameSpecification(username));
        _repository.Remove(user);
        await _repository.SaveChangesAsync();
    }
    
    private User EnsureUserExists(EntitySpecification<User> spec)
    {
        var user = _repository.Find(spec);
        if (user == null)
        {
            throw new ArgumentException("User does not exist");
        }
        return user;
    }
}

