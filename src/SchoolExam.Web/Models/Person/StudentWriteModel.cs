namespace SchoolExam.Web.Models.Person;

public class StudentWriteModel : PersonWriteModel
{
    public Guid SchoolId { get; set; }
}