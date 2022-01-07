using SchoolExam.Domain.Base;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Domain.Entities.PersonAggregate;

public class Person : EntityBase<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Address? Address { get; set; }
    public string EmailAddress { get; set; }
        

    protected Person(Guid id) : base(id)
    {
    }
        
    public Person(Guid id, string firstName, string lastName, DateTime dateOfBirth, Address? address, string emailAddress) : this(id)
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Address = address;
        EmailAddress = emailAddress;
    }
}