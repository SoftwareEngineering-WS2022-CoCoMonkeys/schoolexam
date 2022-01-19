using SchoolExam.Application.Authentication;
using SchoolExam.Application.Repository;
using SchoolExam.Application.Services;
using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.Entities.UserAggregate;
using SchoolExam.Domain.Exceptions;
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

    public async Task<User> Create(string username, string password, Role role, Guid? personId)
    {
        var userId = Guid.NewGuid();
        var user = new User(userId, username,_passwordHasher.HashPassword(password), role, personId);

        _repository.Add(user);
         await _repository.SaveChangesAsync();
         return user;
    }
    
    public async Task<User> CreateFromPerson(Guid personId, string username, string password)
    {
        var person = _repository.Find<Person>(personId);
        if (person == null)
        {
            throw new DomainException("Person does not exist.");
        }

        var userPersonIds = _repository.List<User>().Where(x => x.PersonId.HasValue).Select(x => x.PersonId)
            .ToHashSet();
        if (userPersonIds.Contains(personId))
        {
            throw new DomainException("User connected to person identifier already exists");
        }

        Role role = null!;

        role = person switch
        {
            Student => Role.Student,
            Teacher => Role.Teacher,
            LegalGuardian => Role.LegalGuardian,
            _ => throw new ArgumentException($"{person.GetType().Name} is invalid")
        };
        
        var userId = Guid.NewGuid();
        var user = new User(userId, username, _passwordHasher.HashPassword(password), role, personId);

        _repository.Add(user);
        await _repository.SaveChangesAsync();
        return user;
    }

    public async Task<User> Update(string username, string newUsername, string password, Role role, Guid? personId)
    {
        var user = EnsureUserExists(new UserByUserNameSpecification(username));
 
        user.Username = newUsername;
        user.Password = _passwordHasher.HashPassword(password);
        user.Role = role;
        user.PersonId = personId;
        
        await _repository.SaveChangesAsync();
        return user;
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
            throw new DomainException("User does not exist");
        }
        return user;
    }
}

