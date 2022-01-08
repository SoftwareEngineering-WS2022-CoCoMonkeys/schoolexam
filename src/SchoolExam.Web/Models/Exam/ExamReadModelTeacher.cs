using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Web.Models.Exam;

public class ExamReadModelTeacher
{
    public Guid Id { get; set; }
    public ExamState State { get; set; }
    public string Title { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime Date { get; set; }
    public string Subject { get; set; }
    public int ParticipantCount { get; set; }
    public double? CorrectionProgress { get; set; }
}