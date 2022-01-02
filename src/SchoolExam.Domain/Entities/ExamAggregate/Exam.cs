using SchoolExam.Domain.Base;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Domain.Entities.ExamAggregate;

public class Exam : EntityBase<Guid>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public GradingTable? GradingTable { get; set; }
    public ICollection<ExamTask> Tasks { get; set; }
    public ICollection<ExamBooklet> Booklets { get; set; }
    public DateTime Date { get; set; }
    public Guid CourseId { get; set; }
    public Guid CreatorId { get; set; }
    public TaskPdfFile? TaskPdfFile { get; set; }
    public ExamState State { get; set; }

    protected Exam(Guid id) : base(id)
    {
    }

    public Exam(Guid id, string title, string description, DateTime date, Guid courseId) : this(id)
    {
        Title = title;
        Description = description;
        Date = date;
        CourseId = courseId;
        Tasks = new List<ExamTask>();
        Booklets = new List<ExamBooklet>();
        State = ExamState.Planned;
    }
}