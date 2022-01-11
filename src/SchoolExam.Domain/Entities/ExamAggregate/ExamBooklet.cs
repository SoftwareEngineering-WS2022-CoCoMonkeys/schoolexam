using SchoolExam.Domain.Base;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.Domain.Entities.ExamAggregate;

public class ExamBooklet : EntityBase<Guid>
{
    public Guid ExamId { get; set; }
    public Exam Exam { get; set; }
    public int SequenceNumber { get; set; }
    public BookletPdfFile PdfFile { get; set; }
    public ICollection<ExamBookletPage> Pages { get; set; }
    public Submission? Submission { get; set; }
    public bool HasCompleteSubmission => Pages.All(x => x.IsMatched); 

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