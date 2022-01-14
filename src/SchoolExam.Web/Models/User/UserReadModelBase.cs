using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Web.Models.User;

public class UserReadModelBase
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public Role Role { get; set; }
    public string PersonId { get; set; }
}