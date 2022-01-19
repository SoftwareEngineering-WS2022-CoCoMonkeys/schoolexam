namespace SchoolExam.Application.Services;

public interface ICorrectionService
{
    double GetMaxPoints(Guid examId);
    Task SetGradingTable(Guid examId, params GradingTableIntervalLowerBound[] lowerBounds);
    byte[] GetSubmissionPdf(Guid submissionId);
    byte[] GetRemarkPdf(Guid submissionId);
    Task SetPoints(Guid submissionId, Guid taskId, double? points);
    Task SetRemark(Guid submissionId, byte[] remarkPdf, Guid userId);
}