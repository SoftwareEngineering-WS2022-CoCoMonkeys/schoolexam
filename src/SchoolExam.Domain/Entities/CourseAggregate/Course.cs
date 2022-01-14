using SchoolExam.Domain.Base;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Domain.Entities.CourseAggregate;

public class Course : EntityBase<Guid>
{
    public Topic? Topic { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid SchoolId { get; set; }
    public ICollection<CourseTeacher> Teachers { get; set; }
    public ICollection<CourseStudent> Students { get; set; }

    protected Course(Guid id) : base(id)
    {
    }

    public Course(Guid id, string name, string description, Topic topic, Guid schoolId) : this(id)
    {
        Name = name;
        Description = description;
        Topic = topic;
        SchoolId = schoolId;
        Teachers = new List<CourseTeacher>();
        Students = new List<CourseStudent>();
    }
}