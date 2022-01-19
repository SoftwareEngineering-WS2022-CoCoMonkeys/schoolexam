namespace SchoolExam.Web.Models.User;

public class UserWriteModel
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Role { get; set; } = null!;
    public Guid? PersonId { get; set; }
}