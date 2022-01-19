namespace SchoolExam.Web.Models.Authentication;

public class AuthenticationResultModel
{
    public string Token { get; set; } = null!;
    public AuthenticatedUserModel User { get; set; } = null!;
}