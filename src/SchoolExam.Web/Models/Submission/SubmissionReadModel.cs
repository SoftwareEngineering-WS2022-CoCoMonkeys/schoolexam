using SchoolExam.Web.Models.Exam;

namespace SchoolExam.Web.Models.Submission;

public class SubmissionReadModel
{
    public Guid Id { get; set; }
    public ExamStudentReadModel Student { get; set; }
    public double? AchievedPoints { get; set; }
    public string Status { get; set; }
    public bool IsComplete { get; set; }
    public bool IsMatchedToStudent { get; set; }
    public DateTime UpdatedAt { get; set; }
}