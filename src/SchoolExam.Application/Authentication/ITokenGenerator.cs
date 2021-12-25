using SchoolExam.Domain.Entities.UserAggregate;

namespace SchoolExam.Application.Authentication;

public interface ITokenGenerator
{
    string Generate(User user);
}