namespace SchoolExam.Web.Models.Exam;

public class UnmatchedPagesReadModel
{
    public IEnumerable<UnmatchedBookletPageReadModel> UnmatchedBookletPages { get; set; }
    public IEnumerable<UnmatchedSubmissionPageReadModel> UnmatchedSubmissionPages { get; set; }
}