namespace SchoolExam.Web.Models.Submission;

public class SubmissionDetailsReadModel : SubmissionReadModel
{
    public string Data { get; set; }
    public IEnumerable<AnswerReadModel> Answers { get; set; }
}