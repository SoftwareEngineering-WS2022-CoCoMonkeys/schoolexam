using SchoolExam.Application.RandomGenerator;

namespace SchoolExam.Infrastructure.RandomGenerator;

public class RandomGenerator : IRandomGenerator
{
    private readonly Random _random;
    private readonly string _hexDigits;

    public RandomGenerator(Random random)
    {
        _random = random;
        _hexDigits = "0123456789ABCDEF";
    }

    public string GenerateHexString(int length, bool caps = false)
    {
        if (length < 0)
        {
            throw new ArgumentException("Length of result cannot be negative.");
        }
        var hexDigits = caps ? _hexDigits : _hexDigits.ToLower();
        return new string(Enumerable.Repeat(hexDigits, length).Select(x => x[_random.Next(hexDigits.Length)])
            .ToArray());
    }
}