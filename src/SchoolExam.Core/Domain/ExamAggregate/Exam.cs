using System;
using System.Collections.Generic;
using SchoolExam.Util;

namespace SchoolExam.Core.Domain.ExamAggregate
{
    public class Exam : EntityBase<Guid>
    {
        public GradingTable? GradingTable { get; set; }
        public ICollection<ExamTask> Tasks { get; set; }
        public ICollection<ExamBooklet> Booklets { get; set; }
        public DateTime Date { get; set; }
        public Guid CourseId { get; set; }

        protected Exam(Guid id) : base(id)
        {
        }

        public Exam(Guid id, DateTime date, Guid courseId) : this(id)
        {
            Date = date;
            CourseId = courseId;
            Tasks = new List<ExamTask>();
            Booklets = new List<ExamBooklet>();
        }
    }
}