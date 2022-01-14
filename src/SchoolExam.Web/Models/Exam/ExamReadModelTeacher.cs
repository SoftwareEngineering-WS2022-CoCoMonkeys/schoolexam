using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Web.Models.Exam;

public class ExamReadModelTeacher
{
    public Guid Id { get; set; }
    public ExamState Status { get; set; }
    public string Title { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public string Topic { get; set; }
    public double? Quota { get; set; }
    public IEnumerable<ExamParticipantReadModel> Participants { get; set; }
    public IEnumerable<ExamTaskReadModel> Tasks { get; set; }
}