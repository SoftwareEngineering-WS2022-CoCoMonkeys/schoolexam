namespace SchoolExam.Web.Models.Course;

public class CourseStudentReadModel
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime DateOfBirth { get;set; }
    public string EmailAddress { get; set; } = null!;
}