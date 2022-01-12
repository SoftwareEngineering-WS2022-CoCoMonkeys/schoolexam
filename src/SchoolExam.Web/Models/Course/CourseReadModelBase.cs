namespace SchoolExam.Web.Models.Course;

public abstract class CourseReadModelBase
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string? Topic { get; set; }
}