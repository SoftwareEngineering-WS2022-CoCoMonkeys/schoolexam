using SchoolExam.Domain.Base;

namespace SchoolExam.Domain.Entities.ExamAggregate;

public class ExamBooklet : EntityBase<Guid>
{
    public Guid ExamId { get; set; }
    public int SequenceNumber { get; set; }
    public BookletPdfFile PdfFile { get; set; }
    public ICollection<ExamBookletPage> Pages { get; set; }

    protected ExamBooklet(Guid id) : base(id)
    {
    }

    public ExamBooklet(Guid id, Guid examId, int sequenceNumber, BookletPdfFile pdfFile) : this(id)
    {
        ExamId = examId;
        SequenceNumber = sequenceNumber;
        PdfFile = pdfFile;
        Pages = new List<ExamBookletPage>();
    }
}