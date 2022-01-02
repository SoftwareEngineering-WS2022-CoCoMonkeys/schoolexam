namespace SchoolExam.Domain.ValueObjects;

public enum ExamState
{
    Planned,
    BuildReady,
    SubmissionReady,
    CorrectionReady,
    InCorrection,
    PublishReady,
    Published
}