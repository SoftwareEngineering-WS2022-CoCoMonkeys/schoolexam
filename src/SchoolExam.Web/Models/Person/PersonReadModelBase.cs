using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Web.Models.User;

public class PersonReadModelBase
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Address? Address { get; set; }
    public string EmailAddress { get; set; }
}