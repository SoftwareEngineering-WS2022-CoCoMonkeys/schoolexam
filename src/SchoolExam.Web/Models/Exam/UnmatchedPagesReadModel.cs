namespace SchoolExam.Web.Models.Exam;

public class UnmatchedPagesReadModel
{
    public IEnumerable<UnmatchedBookletPageReadModel> UnmatchedBookletPages { get; set; } = null!;
    public IEnumerable<UnmatchedSubmissionPageReadModel> UnmatchedSubmissionPages { get; set; } = null!;
}