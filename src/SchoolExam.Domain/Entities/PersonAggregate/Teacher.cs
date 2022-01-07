using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Domain.Entities.PersonAggregate;

public class Teacher : Person
{
    public ICollection<CourseTeacher> Courses { get; set; }
    public Guid SchoolId { get; set; }

    protected Teacher(Guid id) : base(id)
    {
    }

    public Teacher(Guid id, string firstName, string lastName, DateTime dateOfBirth, Address address,
        string emailAddress, Guid schoolId) : base(id, firstName, lastName, dateOfBirth, address, emailAddress)
    {
        SchoolId = schoolId;
        Courses = new List<CourseTeacher>();
    }
}