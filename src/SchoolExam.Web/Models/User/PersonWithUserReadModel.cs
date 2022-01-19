using SchoolExam.Web.Models.Person;

namespace SchoolExam.Web.Models.User;

public class PersonWithUserReadModel : PersonReadModel
{
    public UserReadModel User { get; set; } = null!;
}