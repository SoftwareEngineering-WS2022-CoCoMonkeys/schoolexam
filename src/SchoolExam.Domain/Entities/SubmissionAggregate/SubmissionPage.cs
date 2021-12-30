using SchoolExam.Domain.Base;

namespace SchoolExam.Domain.Entities.SubmissionAggregate;

public class SubmissionPage : EntityBase<Guid>
{
    public Guid ExamId { get; set; }
    public SubmissionPagePdfFile PdfFile { get; set; }
    public Guid? SubmissionId { get; set; }
    public int? Page { get; set; }

    protected SubmissionPage(Guid id) : base(id)
    {
    }

    public SubmissionPage(Guid id, Guid examId, SubmissionPagePdfFile pdfFile, Guid? submissionId, int? page) : this(id)
    {
        ExamId = examId;
        PdfFile = pdfFile;
        Page = page;
        SubmissionId = submissionId;
    }
}