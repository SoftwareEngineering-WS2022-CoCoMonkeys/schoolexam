namespace SchoolExam.Web.Models.Exam;

public class UploadTaskPdfModel
{
    public string TaskPdf { get; set; }
    public IEnumerable<ExamTaskModel> Tasks { get; set; }
}