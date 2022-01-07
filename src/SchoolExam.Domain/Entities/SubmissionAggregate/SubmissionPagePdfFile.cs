using SchoolExam.Domain.Base;

namespace SchoolExam.Domain.Entities.SubmissionAggregate;

public class SubmissionPagePdfFile : FileBase
{
    public Guid SubmissionPageId { get; set; }

    public SubmissionPagePdfFile(Guid id) : base(id)
    {
    }

    public SubmissionPagePdfFile(Guid id, string name, long size, DateTime uploadedAt, Guid uploaderId, byte[] content,
        Guid submissionPageId) : base(id, name, size, uploadedAt, uploaderId, content)
    {
        SubmissionPageId = submissionPageId;
    }
}