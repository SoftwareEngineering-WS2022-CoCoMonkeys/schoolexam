using System;

namespace SchoolExam.Core.Domain.ValueObjects
{
    public class ExamPosition : IComparable<ExamPosition>
    {
        public int Page { get; set; }
        public double Y { get; set; }

        public ExamPosition(int page, double y)
        {
            if (page <= 0)
                throw new ArgumentException();
            Page = page;
            if (y < 0 || y > 100)
                throw new ArgumentException();
            Y = y;
        }

        public int CompareTo(ExamPosition? other)
        {
            if (other is null)
                throw new ArgumentException();
            return Page != other.Page ? Page.CompareTo(other.Page) : Y.CompareTo(other.Y);
        }
    }
}