using System;
using SchoolExam.Core.Domain.ValueObjects;
using SchoolExam.SharedKernel;

namespace SchoolExam.Core.Domain.PersonAggregate
{
    public abstract class Person : EntityBase<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Address? Address { get; set; }

        protected Person(Guid id) : base(id)
        {
        }
        
        public Person(Guid id, string firstName, string lastName, DateTime dateOfBirth, Address address) : this(id)
        {
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Address = address;
        }
    }
}