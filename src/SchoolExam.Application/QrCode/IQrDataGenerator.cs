namespace SchoolExam.Application.QrCode;

public interface IQrCodeDataGenerator
{
    string Generate(string course, DateTime date, Guid examBookletId, int page);
}