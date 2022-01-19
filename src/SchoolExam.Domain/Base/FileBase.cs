using SchoolExam.Domain.Entities.UserAggregate;
using SchoolExam.Extensions;

namespace SchoolExam.Domain.Base;

public abstract class FileBase : EntityBase
{
    public string Name { get; set; } = null!;
    public long Size { get; set; }
    public DateTime UploadedAt { get; set; }
    public Guid UploaderId { get; set; }
    public User Uploader { get; set; } = null!;
    public byte[] Content { get; set; } = null!;

    protected FileBase(Guid id) : base(id)
    {
    }

    protected FileBase(Guid id, string name, long size, DateTime uploadedAt, Guid uploaderId, byte[] content) : this(id)
    {
        Name = name;
        Size = size;
        UploadedAt = uploadedAt.SetKindUtc();
        UploaderId = uploaderId;
        Content = content;
    }
}