using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.Entities.UserAggregate;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Application.Services;

public interface IPersonService
{
    Person? GetById(Guid id);

    IEnumerable<Person> GetAllPersons();
    
    Task<Person> Create(string firstName, string lastName, DateTime dateOfBirth, Address? address, string emailAddress);

    Task<User> CreateWithUser(string firstName, string lastName, DateTime dateOfBirth, Address? address, string emailAddress,
        string username, string password, Role role);

    Task<Person> Update(Guid id, string firstName, string lastName, DateTime dateOfBirth, Address? address, string emailAddress);

    Task Delete(Guid id);
}