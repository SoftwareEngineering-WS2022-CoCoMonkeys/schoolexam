using SchoolExam.Domain.Base;
using SchoolExam.Domain.Entities.SubmissionAggregate;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Domain.Entities.ExamAggregate;

public class BookletPage : EntityBase<Guid>
{
    public int Page { get; set; }
    public Guid BookletId { get; set; }
    public QrCode QrCode { get; set; }
    public SubmissionPage? SubmissionPage { get; set; }
    public bool IsMatched => SubmissionPage != null;
        
    protected BookletPage(Guid id) : base(id)
    {
    }

    public BookletPage(Guid id, int page, Guid bookletId, QrCode qrCode) : this(id)
    {
        Page = page;
        BookletId = bookletId;
        QrCode = qrCode;
    }
}