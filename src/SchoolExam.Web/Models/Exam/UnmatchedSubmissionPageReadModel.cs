namespace SchoolExam.Web.Models.Exam;

public class UnmatchedSubmissionPageReadModel
{
    public Guid Id { get; set; }
    public long Size { get; set; }
    public DateTime UploadedAt { get; set; }
    public string UploadedBy { get; set; }
    public Guid FileId { get; set; }
}