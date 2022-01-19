using SchoolExam.Domain.Base;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Domain.Entities.UserAggregate;

public class User : EntityBase
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public Role Role { get; set; } = null!;
    public Guid? PersonId { get; set; }
    public Person Person { get; set; } = null!;

    protected User(Guid id) : base(id)
    {
    }

    public User(Guid id, string username, string password, Role role, Guid? personId) : this(id)
    {
        Username = username;
        Password = password;
        Role = role;
        PersonId = personId;
    }
}