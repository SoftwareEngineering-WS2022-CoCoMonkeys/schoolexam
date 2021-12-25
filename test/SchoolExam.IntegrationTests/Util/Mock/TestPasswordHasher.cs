using SchoolExam.Application.Authentication;

namespace SchoolExam.IntegrationTests.Util.Mock;

public class TestPasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return password;
    }

    public bool VerifyPassword(string password, string hash)
    {
        return password.Equals(hash);
    }
}