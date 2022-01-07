using SchoolExam.Domain.Base;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Domain.Entities.ExamAggregate;

public class ExamBookletPage : EntityBase<Guid>
{
    public int Page { get; set; }
    public Guid BookletId { get; set; }
    public string QrCodeData { get; set; }
    public SubmissionPage? SubmissionPage { get; set; }
    public bool IsMatched => SubmissionPage != null;
        
    protected ExamBookletPage(Guid id) : base(id)
    {
    }

    public ExamBookletPage(Guid id, int page, Guid bookletId, string qrCodeData) : this(id)
    {
        Page = page;
        BookletId = bookletId;
        QrCodeData = qrCodeData;
    }
}