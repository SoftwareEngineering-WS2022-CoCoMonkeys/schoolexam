using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Web.Models.Exam;

public class SubmissionReadModel
{
    public Guid Id { get; set; }
    public ExamStudentReadModel Student { get; set; }
    public string Data { get; set; }
    public IEnumerable<AnswerReadModel> Answers { get; set; }
    public double? AchievedPoints { get; set; }
    public CorrectionState Status { get; set; }
}