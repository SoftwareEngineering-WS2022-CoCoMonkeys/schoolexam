using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Domain.Entities.PersonAggregate;

public class Student : Person
{
    public ICollection<CourseStudent> Courses { get; set; }
    public ICollection<StudentLegalGuardian> LegalGuardians { get; set; }
    public Guid SchoolId { get; set; }
    

    protected Student(Guid id) : base(id)
    {
    }

    public Student(Guid id, string firstName, string lastName, DateTime dateOfBirth, Address address,
        string emailAddress, Guid schoolId) : base(id, firstName, lastName, dateOfBirth, address, emailAddress)
    {
        SchoolId = schoolId;
        Courses = new List<CourseStudent>();
        LegalGuardians = new List<StudentLegalGuardian>();
    }
}