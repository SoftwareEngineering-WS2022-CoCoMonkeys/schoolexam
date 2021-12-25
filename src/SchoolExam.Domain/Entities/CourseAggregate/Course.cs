using SchoolExam.Domain.Base;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Domain.Entities.CourseAggregate;

public class Course : EntityBase<Guid>
{
    public Subject? Subject { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Year { get; set; }
    public Guid SchoolId { get; set; }

    public static string TeachersName => nameof(_teachers);
    private readonly ICollection<CourseTeacher> _teachers;
    public IEnumerable<Guid> TeacherIds => _teachers.Select(x => x.TeacherId);

    public static string StudentsName => nameof(_students);
    private readonly ICollection<CourseStudent> _students;
    public IEnumerable<Guid> StudentIds => _students.Select(x => x.StudentId);

    protected Course(Guid id) : base(id)
    {
    }

    public Course(Guid id, string name, string description, Subject subject, int year, Guid schoolId) : this(id)
    {
        Name = name;
        Description = description;
        Subject = subject;
        Year = year;
        SchoolId = schoolId;
        _teachers = new List<CourseTeacher>();
        _students = new List<CourseStudent>();
    }
}