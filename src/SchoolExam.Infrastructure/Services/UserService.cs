using SchoolExam.Application.Repository;
using SchoolExam.Application.Services;
using SchoolExam.Domain.Entities.UserAggregate;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Infrastructure.Specifications;

namespace SchoolExam.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly ISchoolExamRepository _repository;

    public UserService(ISchoolExamRepository repository)
    {
        _repository = repository;
    }

    public User? GetByUsername(string username)
    {
        return _repository.Find(new UserByUserNameSpecification(username));
    }

    public async Task Update(Guid userId, string username, string password, Role role, Guid personId)
    {
        var user = _repository.Find(new UserByUserNameSpecification(username));
        if (user == null)
        {
            throw new ArgumentException("User does not exist");
        }

        user.Username = username;
        user.Password = password;
        user.Role = role;
        
        
        
        await _repository.SaveChangesAsync();
    }
    
}