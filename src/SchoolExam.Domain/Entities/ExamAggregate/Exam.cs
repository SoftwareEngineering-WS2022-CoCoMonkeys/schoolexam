using SchoolExam.Domain.Base;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Extensions;

namespace SchoolExam.Domain.Entities.ExamAggregate;

public class Exam : EntityBase
{
    public string Title { get; set; }
    public GradingTable? GradingTable { get; set; }
    public ICollection<ExamTask> Tasks { get; set; }
    public ICollection<Booklet> Booklets { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public ICollection<ExamParticipant> Participants { get; set; }
    public Guid CreatorId { get; set; }
    public TaskPdfFile? TaskPdfFile { get; set; }
    public ExamState State { get; set; }
    public Topic Topic { get; set; }

    protected Exam(Guid id) : base(id)
    {
    }

    public Exam(Guid id, string title, DateTime date, Guid creatorId, Topic topic) : this(id)
    {
        Title = title;
        Date = date.SetKindUtc();
        // due date of exam correction is 14 days after exam date
        DueDate = date.AddDays(14);
        CreatorId = creatorId;
        Topic = topic;
        Tasks = new List<ExamTask>();
        Booklets = new List<Booklet>();
        Participants = new List<ExamParticipant>();
        State = ExamState.Planned;
    }

    public double? GetCorrectionProgress()
    {
        var submissions = Booklets.Where(x => x.Submission != null).Select(x => x.Submission!).ToList();
        var submissionCount = submissions.Count;
        var taskCount = Tasks.Count;
        var answerCount = submissions.Sum(x => x.Answers.Count);

        return submissionCount != 0 && taskCount != 0 ? (double) answerCount / (submissionCount * taskCount) : null;
    }
}