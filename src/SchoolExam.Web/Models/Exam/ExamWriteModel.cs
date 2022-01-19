namespace SchoolExam.Web.Models.Exam;

public class ExamWriteModel
{
    public string Title { get; set; } = null!;
    public DateTime Date { get; set; }
    public string Topic { get; set; } = null!;
}