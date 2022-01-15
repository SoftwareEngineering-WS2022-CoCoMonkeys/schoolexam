namespace SchoolExam.Web.Models.Authentication;

public class AuthenticatedPersonModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string EmailAddress { get; set; }
}