using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Persistence.Extensions;

public static class ExamExtensions
{
    public static double? GetCorrectionProgress(this Exam exam)
    {
        var submissions = exam.Booklets.Where(x => x.Submission != null).Select(x => x.Submission!).ToList();
        var submissionCount = submissions.Count;
        var taskCount = exam.Tasks.Count;
        var correctedCount = submissions.Sum(x => x.Answers.Count(a => a.State == AnswerState.Corrected));

        return submissionCount != 0 && taskCount != 0 ? (double) correctedCount / (submissionCount * taskCount) : null;
    }
}