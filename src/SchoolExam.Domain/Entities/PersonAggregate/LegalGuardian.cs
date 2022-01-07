using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Domain.Entities.PersonAggregate;

public class LegalGuardian : Person
{
    public ICollection<StudentLegalGuardian> Children { get; set; }

    protected LegalGuardian(Guid id) : base(id)
    {
    }

    public LegalGuardian(Guid id, string firstName, string lastName, DateTime dateOfBirth, Address address,
        string emailAddress) : base(id, firstName, lastName, dateOfBirth, address, emailAddress)
    {
        Children = new List<StudentLegalGuardian>();
    }
}