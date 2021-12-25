using SchoolExam.Domain.Base;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Domain.Entities.SubmissionAggregate;

public class Submission : EntityBase<Guid>
{
    public double? AchievedPoints => Answers.Sum(x => x.AchievedPoints);
    public CorrectableState State => CorrectableState.Finalized;
    public Guid BookletId { get; set; }
    public Guid StudentId { get; set; }
    public ICollection<Answer> Answers { get; set; }
    public ICollection<SubmissionPage> Pages { get; set; }

    public Submission(Guid id) : base(id)
    {
    }

    public Submission(Guid id, int? achievedPoints, CorrectableState state, Guid studentId, Guid bookletId) :
        this(id)
    {
        StudentId = studentId;
        BookletId = bookletId;
        Answers = new List<Answer>();
        Pages = new List<SubmissionPage>();
    }

    private CorrectableState GetState()
    {
        var count = Answers.Count();
        var correctedCount = Answers.Count(x => x.AchievedPoints != null);
        if (correctedCount == 0)
        {
            return CorrectableState.Pending;
        }

        if (correctedCount > 0 && correctedCount < count)
        {
            return CorrectableState.InProgress;
        }

        if (correctedCount == count)
        {
            return CorrectableState.Finalized;
        }

        throw new Exception("An error occured during computation of correction state.");
    }
}