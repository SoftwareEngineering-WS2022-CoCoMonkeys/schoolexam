using SchoolExam.Domain.Base;
using SchoolExam.Domain.Entities.UserAggregate;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Extensions;

namespace SchoolExam.Domain.Entities.PersonAggregate;

public abstract class Person : EntityBase
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public Address? Address { get; set; }
    public string EmailAddress { get; set; } = null!;
    public User? User { get; set; }

    protected Person(Guid id) : base(id)
    {
    }
        
    protected Person(Guid id, string firstName, string lastName, DateTime dateOfBirth, Address? address, string emailAddress) : this(id)
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth.SetKindUtc();
        Address = address;
        EmailAddress = emailAddress;
    }
}