using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Web.Models.User;

public class UserReadModel
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public Role Role { get; set; }
    public Guid? PersonId { get; set; }
}