using SchoolExam.Domain.Entities.UserAggregate;

namespace SchoolExam.Application.Repositories;

public interface IUserRepository
{
    User? GetByUsername(string username);
}