using SchoolExam.Domain.Base;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Domain.Entities.SchoolAggregate;

public class School : EntityBase
{
    public string Name { get; set; } = null!;
    public Address Location { get; set; } = null!;
    public ICollection<Teacher> Teachers { get; set; }

    protected School(Guid id) : base(id) 
    {
        Teachers = new List<Teacher>();
    }
        
    public School(Guid id, string name, Address location) : this(id)
    {
        Name = name;
        Location = location;
    }
}