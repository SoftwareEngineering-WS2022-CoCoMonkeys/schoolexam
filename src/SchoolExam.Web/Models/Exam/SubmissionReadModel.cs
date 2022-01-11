namespace SchoolExam.Web.Models.Exam;

public class SubmissionReadModel
{
    public Guid Id { get; set; }
    public bool IsMatched { get; set; }
    public string? Student { get; set; }
}