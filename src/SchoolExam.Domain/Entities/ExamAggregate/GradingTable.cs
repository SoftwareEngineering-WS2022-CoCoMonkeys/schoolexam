using SchoolExam.Domain.Base;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Extensions;

namespace SchoolExam.Domain.Entities.ExamAggregate
{
    public class GradingTable : EntityBase<Guid>
    {
        public ICollection<GradingTableInterval> Intervals { get; set; }
        public Guid ExamId { get; set; }

        public GradingTable(Guid id) : base(id)
        {
            Intervals = new List<GradingTableInterval>();
        }
    }
}