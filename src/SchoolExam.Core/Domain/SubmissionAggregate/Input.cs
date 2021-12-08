using System;
using SchoolExam.SharedKernel;

namespace SchoolExam.Core.Domain.SubmissionAggregate
{
    public abstract class Input : EntityBase<Guid>
    {
        protected Input(Guid id) : base(id)
        {
        }
    }
}