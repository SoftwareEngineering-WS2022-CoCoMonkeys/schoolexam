using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Web.Models.Exam;

public class AnswerReadModel
{
    public double? AchievedPoints { get; set; }
    public CorrectionState Status { get; set; }
    public ExamTaskReadModel Task { get; set; }
    public IEnumerable<AnswerSegmentReadModel> Segments { get; set; }
}