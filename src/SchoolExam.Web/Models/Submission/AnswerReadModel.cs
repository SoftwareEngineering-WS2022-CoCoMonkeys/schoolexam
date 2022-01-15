using SchoolExam.Domain.ValueObjects;
using SchoolExam.Web.Models.Exam;

namespace SchoolExam.Web.Models.Submission;

public class AnswerReadModel
{
    public double? AchievedPoints { get; set; }
    public string Status { get; set; }
    public ExamTaskReadModel Task { get; set; }
    public IEnumerable<AnswerSegmentReadModel> Segments { get; set; }
}