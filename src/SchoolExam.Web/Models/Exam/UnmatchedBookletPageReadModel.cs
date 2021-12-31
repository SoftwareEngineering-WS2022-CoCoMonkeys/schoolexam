namespace SchoolExam.Web.Models.Exam;

public class UnmatchedBookletPageReadModel
{
    public Guid Id { get; set; }
    public Guid BookletId { get; set; }
    public int Page { get; set; }
}