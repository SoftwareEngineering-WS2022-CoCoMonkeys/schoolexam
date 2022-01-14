using SchoolExam.Application.TagLayout;

namespace SchoolExam.Application.Services;

public interface IStudentService
{
    byte[] GenerateQrCodeSheetForStudent<TLayout>(Guid studentId) where TLayout : ITagLayout<TLayout>, new();
}