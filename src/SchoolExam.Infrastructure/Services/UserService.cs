using SchoolExam.Application.Repository;
using SchoolExam.Application.Services;
using SchoolExam.Domain.Entities.UserAggregate;
using SchoolExam.Infrastructure.Specifications;

namespace SchoolExam.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly ISchoolExamRepository _context;

    public UserService(ISchoolExamRepository context)
    {
        _context = context;
    }

    public User? GetByUsername(string username)
    {
        return _context.Find(new UserByUserIdSpecification(username));
    }
}