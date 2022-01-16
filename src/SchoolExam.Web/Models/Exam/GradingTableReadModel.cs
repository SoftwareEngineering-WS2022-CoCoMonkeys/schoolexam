namespace SchoolExam.Web.Models.Exam;

public class GradingTableReadModel
{
    public IEnumerable<GradingTableLowerBoundModelBase> LowerBounds { get; set; }
}