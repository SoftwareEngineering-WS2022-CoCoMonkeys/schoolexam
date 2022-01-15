using SchoolExam.Domain.Base;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Domain.Entities.SchoolAggregate;

public class School : EntityBase
{
    public string Name { get; set; }
    public Address Location { get; set; }
    public ICollection<SchoolTeacher> Teachers { get; set; }

    protected School(Guid id) : base(id) 
    {
    }
        
    public School(Guid id, string name, Address location) : this(id)
    {
        Name = name;
        Location = location;
        Teachers = new List<SchoolTeacher>();
    }
}