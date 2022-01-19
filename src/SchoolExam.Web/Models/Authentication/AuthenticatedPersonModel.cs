namespace SchoolExam.Web.Models.Authentication;

public class AuthenticatedPersonModel
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public string EmailAddress { get; set; } = null!;
}