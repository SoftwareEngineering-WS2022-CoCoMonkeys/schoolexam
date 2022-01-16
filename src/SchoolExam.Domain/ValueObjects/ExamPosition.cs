using SchoolExam.Domain.Exceptions;

namespace SchoolExam.Domain.ValueObjects;

public class ExamPosition : IComparable<ExamPosition>
{
    public int Page { get; }
    public double Y { get; }
    
    public ExamPosition(int page, double y)
    {
        if (page <= 0)
            throw new DomainException("Page must not be lower than 1.");
        Page = page;
        if (y < 0)
            throw new DomainException("Y must not be a negative number.");
        Y = y;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;
        if (obj is not ExamPosition other)
            return false;
        return Equals(other);
    }

    protected bool Equals(ExamPosition other)
    {
        return Page == other.Page && Y.Equals(other.Y);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Page, Y);
    }

    public int CompareTo(ExamPosition? other)
    {
        if (other is null)
            throw new ArgumentException();
        return Page != other.Page ? Page.CompareTo(other.Page) : Y.CompareTo(other.Y);
    }
}