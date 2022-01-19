namespace SchoolExam.Web.Models.Person;

public class PersonWriteModel
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public AddressWriteModel? Address { get; set; }
    public string EmailAddress { get; set; } = null!;
}
