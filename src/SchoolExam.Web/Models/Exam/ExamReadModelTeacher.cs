namespace SchoolExam.Web.Models.Exam;

public class ExamReadModelTeacher
{
    public Guid Id { get; set; }
    public string Status { get; set; } = null!;
    public string Title { get; set; } = null!;
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public string Topic { get; set; } = null!;
    public double? Quota { get; set; }
    public GradingTableReadModel? GradingTable { get; set; }
    public IEnumerable<ExamParticipantReadModel> Participants { get; set; } = null!;
    public IEnumerable<ExamTaskReadModel> Tasks { get; set; } = null!;
}