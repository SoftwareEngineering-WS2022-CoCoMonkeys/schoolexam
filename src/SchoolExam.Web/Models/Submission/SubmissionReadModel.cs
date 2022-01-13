using SchoolExam.Domain.ValueObjects;
using SchoolExam.Web.Models.Exam;

namespace SchoolExam.Web.Models.Submission;

public class SubmissionReadModel
{
    public Guid Id { get; set; }
    public ExamStudentReadModel Student { get; set; }
    public double? AchievedPoints { get; set; }
    public CorrectionState Status { get; set; }
}