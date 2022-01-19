namespace SchoolExam.Web.Models.Course;

public abstract class CourseReadModelBase
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Topic { get; set; }
}