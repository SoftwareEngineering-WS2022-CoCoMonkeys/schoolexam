using SchoolExam.Application.QrCode;

namespace SchoolExam.Infrastructure.QrCode;

public class RandomQrCodeDataGenerator : IQrCodeDataGenerator
{
    private readonly Random _random;
    private readonly int _length;
    private readonly string _hexDigits;

    public RandomQrCodeDataGenerator()
    {
        _random = new Random();
        _length = 32;
        _hexDigits = "0123456789ABCDEF";
    }
    
    public string Generate(string course, DateTime date, Guid examBookletId, int page)
    {
        return new string(Enumerable.Repeat(_hexDigits, _length).Select(x => x[_random.Next(_hexDigits.Length)]).ToArray());
    }
}