using SchoolExam.Domain.Base;

namespace SchoolExam.Domain.Entities.ExamAggregate;

public class ExamBookletPage : EntityBase<Guid>
{
    public int Page { get; set; }
    public Guid BookletId { get; set; }
    public string QrCodeData { get; set; }
        
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