namespace SchoolExam.Application.RandomGenerator;

public interface IRandomGenerator
{
    string GenerateHexString(int length, bool caps = false);
}