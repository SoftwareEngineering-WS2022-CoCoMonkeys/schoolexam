using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SchoolExam.Core.Domain.ValueObjects;
using SchoolExam.Util;
using SchoolExam.Util.Extensions;

namespace SchoolExam.Core.Domain.ExamAggregate
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