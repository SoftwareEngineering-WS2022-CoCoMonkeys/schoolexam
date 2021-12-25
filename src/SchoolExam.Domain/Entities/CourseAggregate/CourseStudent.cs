namespace SchoolExam.Domain.Entities.CourseAggregate;

public class CourseStudent
{
    public Guid CourseId { get; set; }
    public Guid StudentId { get; set; }

    public CourseStudent(Guid courseId, Guid studentId)
    {
        CourseId = courseId;
        StudentId = studentId;
    }
}