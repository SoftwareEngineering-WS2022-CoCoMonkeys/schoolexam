using SchoolExam.Domain.Base;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Domain.Entities.ExamAggregate;

public class Booklet : EntityBase
{
    public Guid ExamId { get; set; }
    public Exam Exam { get; set; } = null!;
    public int SequenceNumber { get; set; }
    public BookletPdfFile PdfFile { get; set; } = null!;
    public ICollection<BookletPage> Pages { get; set; }
    public Submission? Submission { get; set; }
    public bool HasCompleteSubmission => Pages.All(x => x.IsMatched); 

    protected Booklet(Guid id) : base(id)
    {
        Pages = new List<BookletPage>();
    }

    public Booklet(Guid id, Guid examId, int sequenceNumber, BookletPdfFile pdfFile) : this(id)
    {
        ExamId = examId;
        SequenceNumber = sequenceNumber;
        PdfFile = pdfFile;
    }
}