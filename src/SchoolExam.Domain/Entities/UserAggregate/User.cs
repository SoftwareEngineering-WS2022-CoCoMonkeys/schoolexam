using SchoolExam.Domain.Base;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Domain.Entities.UserAggregate;

public class User : EntityBase<Guid>
{
    public string Username { get; set; }
    public string Password { get; set; }
    public Role Role { get; set; }
    public Guid? PersonId { get; set; }

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