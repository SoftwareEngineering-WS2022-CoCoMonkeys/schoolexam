using SchoolExam.Application.Repository;
using SchoolExam.Application.Services;
using SchoolExam.Domain.Entities.UserAggregate;
using SchoolExam.Infrastructure.Specifications;

namespace SchoolExam.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly ISchoolExamRepository _repository;

    public UserService(ISchoolExamRepository repository)
    {
        _repository = repository;
    }

    public User? GetByUsername(string username)
    {
        return _repository.Find(new UserByUserIdSpecification(username));
    }
}