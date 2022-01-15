using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Web.Models.User;

public class UserReadModel
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public Role Role { get; set; }
    public string PersonId { get; set; }
}