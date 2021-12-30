using SchoolExam.Domain.Base;

namespace SchoolExam.Domain.Entities.ExamAggregate;

public class ExamBooklet : EntityBase<Guid>
{
    public Guid ExamId { get; set; }
    public ICollection<ExamBookletPage> Pages { get; set; }
    public BookletPdfFile PdfFile { get; set; }

    protected ExamBooklet(Guid id) : base(id)
    {
    }

    public ExamBooklet(Guid id, BookletPdfFile pdfFile, Guid examId) : this(id)
    {
        PdfFile = pdfFile;
        ExamId = examId;
        Pages = new List<ExamBookletPage>();
    }
}