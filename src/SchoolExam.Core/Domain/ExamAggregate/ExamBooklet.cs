using System;
using System.Collections.Generic;
using SchoolExam.Util;

namespace SchoolExam.Core.Domain.ExamAggregate
{
    public class ExamBooklet : EntityBase<Guid>
    {
        public Guid ExamId { get; set; }
        public ICollection<ExamBookletPage> Pages { get; set; }

        protected ExamBooklet(Guid id) : base(id)
        {
        }

        public ExamBooklet(Guid id, Guid examId) : this(id)
        {
            ExamId = examId;
            Pages = new List<ExamBookletPage>();
        }
    }
}