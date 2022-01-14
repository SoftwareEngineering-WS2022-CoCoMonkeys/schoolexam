using SchoolExam.Domain.Base;

namespace SchoolExam.Domain.Entities.SubmissionAggregate;

public class RemarkPdfFile : FileBase
{
    public Guid SubmissionId { get; set; }

    public RemarkPdfFile(Guid id) : base(id)
    {
    }

    public RemarkPdfFile(Guid id, string name, long size, DateTime uploadedAt, Guid uploaderId, byte[] content,
        Guid submissionId) : base(id, name, size, uploadedAt, uploaderId, content)
    {
        SubmissionId = submissionId;
    }
}