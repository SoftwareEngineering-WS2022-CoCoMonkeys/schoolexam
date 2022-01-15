namespace SchoolExam.Web.Models.Authentication;

public class AuthenticatedUserModel
{
    public string Username { get; set; }
    public string Role { get; set; }
    public AuthenticatedPersonModel Person { get; set; }
}