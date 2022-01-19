namespace SchoolExam.Web.Models.Person;

public class TeacherWithUserWriteModel : PersonWithUserWriteModelBase
{
    public Guid SchoolId { get; set; }
}