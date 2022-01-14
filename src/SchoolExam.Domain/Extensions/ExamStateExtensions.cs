using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Domain.Extensions;

public static class ExamStateExtensions
{
    // TODO: move to exam
    public static bool HasBeenBuilt(this ExamState state)
    {
        return state is ExamState.SubmissionReady or ExamState.InCorrection
            or ExamState.Corrected or ExamState.Published;
    }
}