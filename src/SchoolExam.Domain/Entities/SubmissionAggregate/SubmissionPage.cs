using SchoolExam.Domain.Base;

namespace SchoolExam.Domain.Entities.SubmissionAggregate;

public class SubmissionPage : EntityBase<Guid>
{
    public Guid ExamId { get; set; }
    public SubmissionPagePdfFile PdfFile { get; set; }
    public Guid? SubmissionId { get; set; }
    public Guid? BookletPageId { get; set; }
    public bool IsMatched => SubmissionId.HasValue && BookletPageId.HasValue;

    protected SubmissionPage(Guid id) : base(id)
    {
    }

    public SubmissionPage(Guid id, Guid examId, SubmissionPagePdfFile pdfFile, Guid? submissionId, Guid? bookletPageId) : this(id)
    {
        ExamId = examId;
        PdfFile = pdfFile;
        BookletPageId = bookletPageId;
        SubmissionId = submissionId;
    }
}