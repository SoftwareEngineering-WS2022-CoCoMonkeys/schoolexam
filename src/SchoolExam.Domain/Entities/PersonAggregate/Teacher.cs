using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Domain.Entities.PersonAggregate;

public class Teacher : Person
{
    public static string CoursesName = nameof(_courses);
    private readonly ICollection<CourseTeacher> _courses;
    public IEnumerable<Guid> CourseIds => _courses.Select(x => x.CourseId);

    public Guid SchoolId { get; set; }

    protected Teacher(Guid id) : base(id)
    {
    }

    public Teacher(Guid id, string firstName, string lastName, DateTime dateOfBirth, Address address,
        string emailAddress, Guid schoolId) : base(id, firstName, lastName, dateOfBirth, address, emailAddress)
    {
        SchoolId = schoolId;
        _courses = new List<CourseTeacher>();
    }

    public override Role GetRole() => Role.Teacher;
}