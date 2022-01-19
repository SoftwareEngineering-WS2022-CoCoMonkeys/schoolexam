using System.Threading.Tasks;
using SchoolExam.Application.Repository;

namespace SchoolExam.IntegrationTests.Util.Mock;

public class TestSchoolExamRepositoryInitService : ISchoolExamRepositoryInitService
{
    public Task Init(bool reset)
    {
        return Task.CompletedTask;
    }
}