namespace SchoolExam.Core.UserManagement.UserAggregate;

public interface IUserRepository
{
    User? GetByUsername(string username);
}