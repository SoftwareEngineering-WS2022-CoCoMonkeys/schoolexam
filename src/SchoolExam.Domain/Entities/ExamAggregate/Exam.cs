using SchoolExam.Domain.Base;

namespace SchoolExam.Domain.Entities.ExamAggregate;

public class Exam : EntityBase<Guid>
{
    public GradingTable? GradingTable { get; set; }
    public ICollection<ExamTask> Tasks { get; set; }
    public ICollection<ExamBooklet> Booklets { get; set; }
    public DateTime Date { get; set; }
    public Guid CourseId { get; set; }
    public Guid CreatorId { get; set; }

    protected Exam(Guid id) : base(id)
    {
    }

    public Exam(Guid id, DateTime date, Guid courseId) : this(id)
    {
        Date = date;
        CourseId = courseId;
        Tasks = new List<ExamTask>();
        Booklets = new List<ExamBooklet>();
    }
}