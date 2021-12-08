using System;
using SchoolExam.Core.Domain.ValueObjects;
using SchoolExam.SharedKernel;

namespace SchoolExam.Core.Domain.ExamAggregate
{
    public class ExamTask : EntityBase<Guid>
    {
        public Guid TaskId { get; set; }
        public int MaxPoints { get; set; }
        public int Number { get; set; }
        public ExamTaskPosition Position { get; set; }

        public ExamTask(Guid id, Guid taskId, int maxPoints, int number) : base(id)
        {
            TaskId = taskId;
            MaxPoints = maxPoints;
            Number = number;
        }

        public ExamTask(Guid id, Guid taskId, int maxPoints, int number, ExamTaskPosition position) : this(id, taskId,
            maxPoints, number)
        {
            Position = position;
        }
    }
}