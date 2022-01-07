using SchoolExam.Domain.Base;

namespace SchoolExam.Domain.Entities.ExamAggregate;

public class BookletPdfFile : FileBase
{
    public Guid BookletId { get; set; }

    public BookletPdfFile(Guid id) : base(id)
    {
    }

    public BookletPdfFile(Guid id, string name, long size, DateTime uploadedAt, Guid uploaderId, byte[] content,
        Guid bookletId) : base(id, name, size, uploadedAt, uploaderId, content)
    {
        BookletId = bookletId;
    }
}