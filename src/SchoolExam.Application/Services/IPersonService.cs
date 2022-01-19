using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Application.Services;

public interface IPersonService
{
    Person? GetById(Guid id);

    IEnumerable<Person> GetAllPersons();

    Task<Student> CreateStudent(string firstName, string lastName, DateTime dateOfBirth, Address? address,
        string emailAddress, Guid schoolId);

    Task<Teacher> CreateTeacher(string firstName, string lastName, DateTime dateOfBirth, Address? address,
        string emailAddress, Guid schoolId);

    Task<LegalGuardian> CreateLegalGuardian(string firstName, string lastName, DateTime dateOfBirth, Address? address,
        string emailAddress);

    Task<Person> Update(Guid id, string firstName, string lastName, DateTime dateOfBirth, Address? address, string emailAddress);

    Task Delete(Guid id);
}