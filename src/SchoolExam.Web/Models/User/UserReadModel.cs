namespace SchoolExam.Web.Models.User;

public class UserReadModel
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Role { get; set; } = null!;
    public Guid? PersonId { get; set; }
}