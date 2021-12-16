using SchoolExam.Application.QrCode;

namespace SchoolExam.Infrastructure.QrCode;

public class QrCodeDataGenerator : IQrCodeDataGenerator
{
    public string Generate(string course, DateTime date, Guid examBookletId, int page)
    {
        return $"{course}-{date:yyyy-MM-dd}-{examBookletId}-{page}";
    }
}