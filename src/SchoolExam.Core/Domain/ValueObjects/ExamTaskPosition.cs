using System;

namespace SchoolExam.Core.Domain.ValueObjects
{
    public class ExamTaskPosition
    {
        public ExamPosition Start { get; set; }
        public ExamPosition End { get; set; }

        public ExamTaskPosition()
        {
        }

        public ExamTaskPosition(ExamPosition start, ExamPosition end)
        {
            if (start.CompareTo(end) > 0)
                throw new ArgumentException();
            Start = start;
            End = end;
        }
    }
}