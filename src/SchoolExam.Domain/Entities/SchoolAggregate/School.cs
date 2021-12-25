using SchoolExam.Domain.Base;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Domain.Entities.SchoolAggregate;

public class School : EntityBase<Guid>
{
    public string Name { get; set; }
    public Address Location { get; set; }
    public bool HasScanner { get; set; }

    public static string TeachersName = nameof(_teachers);
    private readonly ICollection<SchoolTeacher> _teachers;
    public IEnumerable<Guid> TeacherIds => _teachers.Select(x => x.TeacherId);

    protected School(Guid id) : base(id) 
    {
    }
        
    public School(Guid id, string name, Address location, bool hasScanner) : this(id)
    {
        Name = name;
        Location = location;
        HasScanner = hasScanner;
        _teachers = new List<SchoolTeacher>();
    }
}