using System;
using System.Collections;
using System.Collections.Generic;

namespace SchoolExam.Core.Domain.SubmissionAggregate
{
    public class Submission : Correctable
    {
        public bool IsDigital { get; set; }
        public Guid BookletId { get; set; }
        public Guid StudentId { get; set; }
        public IEnumerable<Answer> Answers { get; set; }
        public IEnumerable<SubmissionPage> Pages { get; set; }

        public Submission(Guid id, int? achievedPoints, CorrectableState state, bool isDigital, Guid studentId, Guid bookletId) : base(
            id, achievedPoints, state)
        {
            IsDigital = isDigital;
            StudentId = studentId;
            BookletId = bookletId;
            Answers = new List<Answer>();
            Pages = new List<SubmissionPage>();
        }
    }
}