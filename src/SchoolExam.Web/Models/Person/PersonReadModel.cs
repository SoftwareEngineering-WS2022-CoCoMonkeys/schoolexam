using SchoolExam.Domain.ValueObjects;
using SchoolExam.Web.Models.User;

namespace SchoolExam.Web.Models.Person;

public class PersonReadModel
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public AddressReadModel? Address { get; set; }
    public string EmailAddress { get; set; }
    
}