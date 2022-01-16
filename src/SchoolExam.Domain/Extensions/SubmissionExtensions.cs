using SchoolExam.Domain.Entities.SubmissionAggregate;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Domain.Extensions;

public static class SubmissionExtensions
{
    public static CorrectionState GetCorrectionState(this Submission submission)
    {
        var countCorrected = submission.Answers.Count(x => x.State == AnswerState.Corrected);
        var countPending = submission.Answers.Count(x => x.State == AnswerState.Pending);
        var count = submission.Answers.Count;

        if (count == countPending)
        {
            return CorrectionState.Pending;
        }

        if (count == countCorrected)
        {
            return CorrectionState.Corrected;
        }

        return CorrectionState.InProgress;
    }
}