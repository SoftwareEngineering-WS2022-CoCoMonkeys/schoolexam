using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Domain.Entities.PersonAggregate;

public class LegalGuardian : Person
{
    public static string ChildrenName = nameof(_children);
    private readonly ICollection<Student> _children;
    public IEnumerable<Guid> ChildIds => _children.Select(x => x.Id);

    protected LegalGuardian(Guid id) : base(id)
    {
    }

    public LegalGuardian(Guid id, string firstName, string lastName, DateTime dateOfBirth, Address address,
        string emailAddress) : base(id, firstName, lastName, dateOfBirth, address, emailAddress)
    {
        _children = new List<Student>();
    }

    public override Role GetRole() => Role.LegalGuardian;
}