using SchoolExam.Domain.Entities.UserAggregate;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Application.Services;

public interface IUserService
{
    User? GetByUsername(string username);
    
    User? GetById(Guid id);

    IEnumerable<User> GetAllUsers();

    Task Create(string username, string password, Role role, Guid? personId);

    Task Update( string username, string password, Role role, Guid? personId);

    Task Delete(String userName);


}