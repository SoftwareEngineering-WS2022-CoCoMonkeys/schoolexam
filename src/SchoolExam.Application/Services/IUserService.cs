using SchoolExam.Domain.Entities.UserAggregate;

namespace SchoolExam.Application.Services;

public interface IUserService
{
    User? GetByUsername(string username);
}