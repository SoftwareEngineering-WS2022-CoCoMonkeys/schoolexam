using SchoolExam.Application.TagLayout;

namespace SchoolExam.Application.Services;

public interface IExamBuildService
{
    Task<int> Build(Guid examId, Guid userId);
    Task Clean(Guid examId);
    byte[] GetParticipantQrCodePdf<TLayout>(Guid examId) where TLayout : ITagLayout<TLayout>, new();
    byte[] GetConcatenatedBookletPdfFile(Guid examId);
}