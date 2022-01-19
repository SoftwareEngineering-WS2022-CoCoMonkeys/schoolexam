namespace SchoolExam.Web.Models.Course;

public class CourseReadModelTeacher : CourseReadModelBase
{
    public IEnumerable<CourseStudentReadModel> Students { get; set; }
}