using SchoolExam.Domain.Base;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.PersonAggregate;

namespace SchoolExam.Domain.Entities.SubmissionAggregate;

public class Submission : EntityBase<Guid>
{
    public Guid BookletId { get; set; }
    public Booklet Booklet { get; set; }
    public Guid? StudentId { get; set; }
    public Student? Student { get; set; }
    public ICollection<Answer> Answers { get; set; }
    public ICollection<SubmissionPage> Pages { get; set; }
    public SubmissionPdfFile? PdfFile { get; set; }

    public Submission(Guid id) : base(id)
    {
    }

    public Submission(Guid id, Guid? studentId, Guid bookletId) :
        this(id)
    {
        StudentId = studentId;
        BookletId = bookletId;
        Answers = new List<Answer>();
        Pages = new List<SubmissionPage>();
    }
}