namespace SchoolExam.Domain.Entities.CourseAggregate;

public class CourseTeacher
{
    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public Guid TeacherId { get; set; }

    public CourseTeacher(Guid courseId, Guid teacherId)
    {
        CourseId = courseId;
        TeacherId = teacherId;
    }
}