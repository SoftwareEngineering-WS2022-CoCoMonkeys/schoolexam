using SchoolExam.Application.RandomGenerator;
using SchoolExam.Application.Repository;
using SchoolExam.Application.Services;
using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.Exceptions;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Infrastructure.Extensions;
using SchoolExam.Infrastructure.Specifications;

namespace SchoolExam.Infrastructure.Services;

public class PersonService : IPersonService
{
    private readonly ISchoolExamRepository _repository;
    private readonly IRandomGenerator _randomGenerator;

    public PersonService(ISchoolExamRepository repository, IRandomGenerator randomGenerator)
    {
        _repository = repository;
        _randomGenerator = randomGenerator;
    }

    public Person? GetById(Guid id)
    {
        return _repository.Find(new PersonByIdSpecification(id));
    }

    public IEnumerable<Person> GetAllPersons()
    {
        return _repository.List<Person>();
    }

    public async Task<Student> CreateStudent(string firstName, string lastName, DateTime dateOfBirth, Address? address,
        string emailAddress, Guid schoolId)
    {
        var studentId = Guid.NewGuid();
        var qrCodeData = _randomGenerator.GenerateHexString(32);
        var student = new Student(studentId, firstName, lastName, dateOfBirth, address, emailAddress, qrCodeData,
            schoolId);

        _repository.Add(student);
        await _repository.SaveChangesAsync();
        return student;
    }

    public async Task<Teacher> CreateTeacher(string firstName, string lastName, DateTime dateOfBirth, Address? address,
        string emailAddress, Guid schoolId)
    {
        var studentId = Guid.NewGuid();
        var teacher = new Teacher(studentId, firstName, lastName, dateOfBirth, address, emailAddress, schoolId);

        _repository.Add(teacher);
        await _repository.SaveChangesAsync();
        return teacher;
    }

    public async Task<LegalGuardian> CreateLegalGuardian(string firstName, string lastName, DateTime dateOfBirth,
        Address? address, string emailAddress)
    {
        var studentId = Guid.NewGuid();
        var legalGuardian = new LegalGuardian(studentId, firstName, lastName, dateOfBirth, address, emailAddress);

        _repository.Add(legalGuardian);
        await _repository.SaveChangesAsync();
        return legalGuardian;
    }

    public async Task<Person> Update(Guid id, string firstName, string lastName, DateTime dateOfBirth, Address? address,
        string emailAddress)
    {
        var person = EnsurePersonExists(new PersonByIdSpecification(id));

        person.FirstName = firstName;
        person.LastName = lastName;
        person.DateOfBirth = dateOfBirth;
        person.Address = address;
        person.EmailAddress = emailAddress;
        await _repository.SaveChangesAsync();
        return person;
    }

    public async Task Delete(Guid id)
    {
        var person = EnsurePersonExists(new PersonByIdSpecification(id));
        _repository.Remove(person);
        await _repository.SaveChangesAsync();
    }

    private Person EnsurePersonExists(EntityByIdSpecification<Person> spec)
    {
        var person = _repository.Find(spec);
        if (person == null)
        {
            throw new DomainException("Person does not exist.");
        }

        return person;
    }
}