using SchoolExam.Domain.Base;
using SchoolExam.Domain.Entities.CourseAggregate;
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
    public DateTime DueDate { get; set; }
    public Guid CourseId { get; set; }
    public Course Course { get; set; }
    public Guid CreatorId { get; set; }
    public TaskPdfFile? TaskPdfFile { get; set; }
    public ExamState State { get; set; }

    protected Exam(Guid id) : base(id)
    {
    }

    public Exam(Guid id, string title, string description, DateTime date, Guid creatorId, Guid courseId) : this(id)
    {
        Title = title;
        Description = description;
        Date = date;
        // due date of exam correction is 14 days after exam date
        DueDate = date.AddDays(14);
        CourseId = courseId;
        CreatorId = creatorId;
        Tasks = new List<ExamTask>();
        Booklets = new List<ExamBooklet>();
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