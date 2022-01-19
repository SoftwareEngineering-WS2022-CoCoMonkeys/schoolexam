using SchoolExam.Web.Models.Exam;

namespace SchoolExam.Web.Models.Submission;

public class SubmissionReadModel
{
    public Guid Id { get; set; }
    public ExamStudentReadModel Student { get; set; } = null!;
    public double? AchievedPoints { get; set; }
    public string Status { get; set; } = null!;
    public bool IsComplete { get; set; }
    public bool IsMatchedToStudent { get; set; }
    public DateTime UpdatedAt { get; set; }
}