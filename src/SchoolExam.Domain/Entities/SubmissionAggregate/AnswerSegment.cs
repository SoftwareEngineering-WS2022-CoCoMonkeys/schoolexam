using SchoolExam.Domain.Base;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Domain.Entities.SubmissionAggregate;

public class AnswerSegment : EntityBase
{
    public ExamPosition Start { get; set; } = null!;
    public ExamPosition End { get; set; } = null!;
    public Guid AnswerId { get; set; }

    protected AnswerSegment(Guid id) : base(id)
    {
    }

    public AnswerSegment(Guid id, ExamPosition start, ExamPosition end, Guid answerId) : this(id)
    {
        Start = start;
        End = end;
        AnswerId = answerId;
    }
}