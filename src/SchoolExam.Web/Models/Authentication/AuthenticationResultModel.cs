namespace SchoolExam.Web.Models.Authentication;

public class AuthenticationResultModel
{
    public string Token { get; set; }
    public AuthenticatedUserModel User { get; set; }
}