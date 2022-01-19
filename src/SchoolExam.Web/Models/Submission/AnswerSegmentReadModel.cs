namespace SchoolExam.Web.Models.Submission;

public class AnswerSegmentReadModel
{
    public SegmentPositionReadModel Start { get; set; } = null!;
    public SegmentPositionReadModel End { get; set; } = null!;
}