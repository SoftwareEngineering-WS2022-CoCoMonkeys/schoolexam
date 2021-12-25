using SchoolExam.Domain.Base;

namespace SchoolExam.Domain.Entities.ExamAggregate;

public class ExamBooklet : EntityBase<Guid>
{
    public Guid ExamId { get; set; }
    public ICollection<ExamBookletPage> Pages { get; set; }

    protected ExamBooklet(Guid id) : base(id)
    {
    }

    public ExamBooklet(Guid id, Guid examId) : this(id)
    {
        ExamId = examId;
        Pages = new List<ExamBookletPage>();
    }
}