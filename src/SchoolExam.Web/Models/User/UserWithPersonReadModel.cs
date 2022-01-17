using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Web.Models.User;

public class UserWithPersonReadModel
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public Role Role { get; set; }
    public Domain.Entities.PersonAggregate.Person Person { get; set; }
}