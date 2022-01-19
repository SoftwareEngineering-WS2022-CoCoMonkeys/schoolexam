namespace SchoolExam.Web.Models.Authentication;

public class AuthenticatedUserModel
{
    public string Username { get; set; } = null!;
    public string Role { get; set; } = null!;
    public AuthenticatedPersonModel? Person { get; set; }
}