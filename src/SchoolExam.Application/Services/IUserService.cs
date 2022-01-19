using SchoolExam.Domain.Entities.UserAggregate;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Application.Services;

public interface IUserService
{
    User? GetByUsername(string username);
    
    User? GetById(Guid id);

    IEnumerable<User> GetAllUsers();

    Task<User> Create(string username, string password, Role role, Guid? personId);
    
    Task<User> CreateFromPerson(Guid personId, string username, string password);

    Task<User> Update( string username, string newUsername, string password, Role role, Guid? personId);

    Task Delete(String userName);


}