using SchoolExam.Util;

namespace SchoolExam.Core.UserManagement.UserAggregate;

public class User : EntityBase<Guid>
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public Guid? PersonId { get; set; }

    protected User(Guid id) : base(id)
    {
    }

    public User(Guid id, string username, string password, string role, Guid? personId) : this(id)
    {
        Username = username;
        Password = password;
        Role = role;
        PersonId = personId;
    }
}