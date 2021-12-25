namespace SchoolExam.Web.Course;

public abstract class CourseReadModelBase
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Year { get; set; }
    public string? Subject { get; set; }
}