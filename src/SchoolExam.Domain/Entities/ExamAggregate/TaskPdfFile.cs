using SchoolExam.Domain.Base;

namespace SchoolExam.Domain.Entities.ExamAggregate;

public class TaskPdfFile : FileBase
{
    public Guid ExamId { get; set; }

    public TaskPdfFile(Guid id) : base(id)
    {
    }

    public TaskPdfFile(Guid id, string name, long size, DateTime uploadedAt, Guid uploaderId, byte[] content,
        Guid examId) : base(id, name, size, uploadedAt, uploaderId, content)
    {
        ExamId = examId;
    }
}