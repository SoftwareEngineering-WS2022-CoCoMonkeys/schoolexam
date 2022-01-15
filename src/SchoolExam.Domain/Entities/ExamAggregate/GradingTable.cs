using SchoolExam.Domain.Base;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Domain.Entities.ExamAggregate
{
    public class GradingTable : EntityBase
    {
        public ICollection<GradingTableInterval> Intervals { get; set; }
        public Guid ExamId { get; set; }

        public GradingTable(Guid id, Guid examId) : base(id)
        {
            Intervals = new List<GradingTableInterval>();
            ExamId = examId;
        }
    }
}