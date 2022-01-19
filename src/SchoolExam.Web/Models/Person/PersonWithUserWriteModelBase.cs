namespace SchoolExam.Web.Models.Person;

public abstract class PersonWithUserWriteModelBase
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public AddressWriteModel? Address { get; set; }
    public string EmailAddress { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}