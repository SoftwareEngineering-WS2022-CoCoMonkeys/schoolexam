namespace SchoolExam.Web.Models.Person;

public class StudentWithUserWriteModel : PersonWithUserWriteModelBase
{
    public Guid SchoolId { get; set; }
}