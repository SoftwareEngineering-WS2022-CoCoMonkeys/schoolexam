using SchoolExam.Web.Models.Exam;

namespace SchoolExam.Web.Models.Submission;

public class AnswerReadModel
{
    public double? AchievedPoints { get; set; }
    public string Status { get; set; } = null!;
    public DateTime UpdatedAt { get; set; }
    public ExamTaskReadModel Task { get; set; } = null!;
    public IEnumerable<AnswerSegmentReadModel> Segments { get; set; } = null!;
}