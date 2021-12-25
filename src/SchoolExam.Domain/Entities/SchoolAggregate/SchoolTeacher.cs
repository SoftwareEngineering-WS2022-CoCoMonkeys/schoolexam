namespace SchoolExam.Domain.Entities.SchoolAggregate;

public class SchoolTeacher
{
    public Guid SchoolId { get; set; }
    public Guid TeacherId { get; set; }

    public SchoolTeacher(Guid schoolId, Guid teacherId)
    {
        SchoolId = schoolId;
        TeacherId = teacherId;
    }
}