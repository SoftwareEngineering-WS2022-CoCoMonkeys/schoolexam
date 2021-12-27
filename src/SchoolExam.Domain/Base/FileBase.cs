namespace SchoolExam.Domain.Base;

public abstract class FileBase : EntityBase<Guid>
{
    public string Name { get; set; }
    public long Size { get; set; }
    public DateTime UploadedAt { get; set; }
    public Guid UploaderId { get; set; }
    public byte[] Content { get; set; }

    protected FileBase(Guid id) : base(id)
    {
    }

    protected FileBase(Guid id, string name, long size, DateTime uploadedAt, Guid uploaderId, byte[] content) : this(id)
    {
        Name = name;
        Size = size;
        UploadedAt = uploadedAt;
        UploaderId = uploaderId;
        Content = content;
    }
}