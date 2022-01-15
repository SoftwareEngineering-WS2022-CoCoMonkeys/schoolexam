using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Web.Models.User;

public class UserWriteModel
{
    public string Password { get; set; }
    public string Role { get; set; }
    public string? PersonId { get; set; }
}