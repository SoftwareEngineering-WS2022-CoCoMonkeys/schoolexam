namespace SchoolExam.Web.Models.Exam;

public class ExamTaskWriteModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public double MaxPoints { get; set; }
}