using System;
using SchoolExam.Util;

namespace SchoolExam.Core.Domain.SubmissionAggregate
{
    public abstract class Input : EntityBase<Guid>
    {
        protected Input(Guid id) : base(id)
        {
        }
    }
}