using SchoolExam.Domain.Entities.PersonAggregate;

namespace SchoolExam.Domain.Entities.CourseAggregate;

public class CourseStudent
{
    public Course Course { get; set; }
    public Guid CourseId { get; set; }
    public Student Student { get; set; }
    public Guid StudentId { get; set; }

    public CourseStudent(Guid courseId, Guid studentId)
    {
        CourseId = courseId;
        StudentId = studentId;
    }
}