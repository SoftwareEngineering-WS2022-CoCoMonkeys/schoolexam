using SchoolExam.Domain.Base;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Extensions;

namespace SchoolExam.Domain.Entities.ExamAggregate;

public class Exam : EntityBase
{
    public string Title { get; set; } = null!;
    public GradingTable? GradingTable { get; set; }
    public ICollection<ExamTask> Tasks { get; set; }
    public ICollection<Booklet> Booklets { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public ICollection<ExamParticipant> Participants { get; set; }
    public Guid CreatorId { get; set; }
    public TaskPdfFile? TaskPdfFile { get; set; }
    public ExamState State { get; set; }
    public Topic Topic { get; set; } = null!;

    protected Exam(Guid id) : base(id)
    {
        Tasks = new List<ExamTask>();
        Booklets = new List<Booklet>();
        Participants = new List<ExamParticipant>();
    }

    public Exam(Guid id, string title, DateTime date, Guid creatorId, Topic topic) : this(id)
    {
        Title = title;
        Date = date.SetKindUtc();
        // due date of exam correction is 14 days after exam date
        DueDate = date.AddDays(14);
        CreatorId = creatorId;
        Topic = topic;
        State = ExamState.Planned;
    }
}