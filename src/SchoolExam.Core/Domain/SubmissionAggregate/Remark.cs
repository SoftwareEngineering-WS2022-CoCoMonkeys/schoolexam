using System;
using SchoolExam.SharedKernel;

namespace SchoolExam.Core.Domain.SubmissionAggregate
{
    public class Remark : EntityBase<Guid>
    {
        public Input Input { get; set; }
        public DateTime DateTime { get; set; }
        public Guid TeacherId { get; set; }

        protected Remark(Guid id) : base(id)
        {
        }

        public Remark(Guid id, DateTime dateTime, Guid teacherId, Input input) : this(id)
        {
            DateTime = dateTime;
            TeacherId = teacherId;
            Input = input;
        }
    }
}