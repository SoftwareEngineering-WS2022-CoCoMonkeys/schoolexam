using SchoolExam.Domain.Base;

namespace SchoolExam.Domain.Entities.SubmissionAggregate;

public class SubmissionPdfFile : FileBase
{
    public Guid SubmissionId { get; set; }

    public SubmissionPdfFile(Guid id) : base(id)
    {
    }

    public SubmissionPdfFile(Guid id, string name, long size, DateTime uploadedAt, Guid uploaderId, byte[] content,
        Guid submissionId) : base(id, name, size, uploadedAt, uploaderId, content)
    {
        SubmissionId = submissionId;
    }
}