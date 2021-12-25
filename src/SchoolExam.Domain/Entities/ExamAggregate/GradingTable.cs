using SchoolExam.Domain.Base;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Extensions;

namespace SchoolExam.Domain.Entities.ExamAggregate
{
    public class GradingTable : EntityBase<Guid>
    {
        private readonly ICollection<GradingTableInterval> intervals;

        public IReadOnlyCollection<GradingTableInterval> Intervals => intervals.AsReadOnly();

        public GradingTable(Guid id) : base(id)
        {
            intervals = new List<GradingTableInterval>();
        }

        public double GetGrade(int points)
        {
            return intervals.Single(x => x.Includes(points)).Grade;
        }

        public void AddInterval(GradingTableInterval interval)
        {
            if (intervals.Any(x => x.Overlaps(interval)))
                throw new ArgumentException();
            intervals.Add(interval);
        }
    }
}