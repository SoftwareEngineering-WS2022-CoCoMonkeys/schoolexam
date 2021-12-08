using System;
using System.Collections.Generic;
using SchoolExam.SharedKernel;

namespace SchoolExam.Core.Domain.SubmissionAggregate
{
    public abstract class Correctable : EntityBase<Guid>
    {
        public CorrectableState State { get; set; }
        public int? AchievedPoints { get; set; }
        public IEnumerable<Remark> Remarks { get; set; }

        protected Correctable(Guid id) : base(id)
        {
        }

        public Correctable(Guid id, int? achievedPoints, CorrectableState state) : this(id)
        {
            AchievedPoints = achievedPoints;
            State = state;
            Remarks = new List<Remark>();
        }
    }
}