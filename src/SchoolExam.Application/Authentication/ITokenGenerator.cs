using SchoolExam.Core.UserManagement.UserAggregate;

namespace SchoolExam.Application.Authentication;

public interface ITokenGenerator
{
    string Generate(User user);
}