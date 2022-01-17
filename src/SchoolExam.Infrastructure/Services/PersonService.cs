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

public class PersonService : IPersonService
{
    private readonly ISchoolExamRepository _context;
    private readonly IUserService _userService;

    public PersonService(ISchoolExamRepository context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }
    
    public Person? GetById(Guid id)
    {
        return _context.Find(new PersonByIdSpecification(id));
    }

    public IEnumerable<Person> GetAllPersons()
    {
        return _context.List<Person>();
    }

    public async Task<Person> Create(string firstName, string lastName, DateTime dateOfBirth, Address? address, string emailAddress)
    {
        var personId = Guid.NewGuid();
        var person = new Person(personId, firstName, lastName, dateOfBirth, address, emailAddress);

        _context.Add(person);
        await _context.SaveChangesAsync();
        return person;
    }

    public async Task<User> CreateWithUser(string firstName, string lastName, DateTime dateOfBirth, Address? address,
        string emailAddress, string username, string password, Role role)
    {
        var personId = Guid.NewGuid();
        var person = new Person(personId, firstName, lastName, dateOfBirth, address, emailAddress);

        _context.Add(person);
        await _context.SaveChangesAsync();

        var user = await _userService.Create(username, password, role, personId);

        return user;
    }

    public async Task<Person> Update(Guid id, string firstName, string lastName, DateTime dateOfBirth, Address address,
        string emailAddress)
    {
        var person = EnsurePersonExists(new PersonByIdSpecification(id));

        person.FirstName = firstName;
        person.LastName = lastName;
        person.DateOfBirth = dateOfBirth;
        person.Address = address;
        person.EmailAddress = emailAddress;
        await _context.SaveChangesAsync();
        return person;
    }

    public async Task Delete(Guid id)
    {
        var person = EnsurePersonExists(new PersonByIdSpecification(id));
        _context.Remove(person);
        await _context.SaveChangesAsync();
    }
    
    private Person EnsurePersonExists(EntityByIdSpecification<Person> spec)
    {
        var person = _context.Find(spec);
        if (person == null)
        {
            throw new DomainException("Person does not exist");
        }

        return person;
    }
}