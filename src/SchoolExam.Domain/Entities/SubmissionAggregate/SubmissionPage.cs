using SchoolExam.Domain.Base;

namespace SchoolExam.Domain.Entities.SubmissionAggregate;

public class SubmissionPage : EntityBase<Guid>
{
    public int Page { get; set; }
    public byte[] ScanData { get; set; }
        
    protected SubmissionPage(Guid id) : base(id)
    {
    }

    public SubmissionPage(Guid id, int page, byte[] scanData) : this(id)
    {
        Page = page;
        ScanData = scanData;
    }
}