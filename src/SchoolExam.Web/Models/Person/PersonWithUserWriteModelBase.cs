namespace SchoolExam.Web.Models.Person;

public abstract class PersonWithUserWriteModelBase
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public AddressWriteModel? Address { get; set; }
    public string EmailAddress { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}