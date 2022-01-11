namespace SchoolExam.Application.Services;

public interface IStudentService
{
    byte[] GetQrCodeByStudentId(Guid studentId);
}