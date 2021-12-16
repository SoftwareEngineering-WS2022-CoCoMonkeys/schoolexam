using System;
using SchoolExam.Util;

namespace SchoolExam.Core.Domain.TaskAggregate
{
    public class Task : EntityBase<Guid>
    {
        public TaskType Type { get; set; }
        public string Description { get; set; }
        
        public Task(Guid id, TaskType type, string description) : base(id)
        {
            Type = type;
            Description = description;
        }
    }
}