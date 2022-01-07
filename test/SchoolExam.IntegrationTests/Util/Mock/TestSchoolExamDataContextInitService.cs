using System.Threading.Tasks;
using SchoolExam.Application.DataContext;

namespace SchoolExam.IntegrationTests.Util.Mock;

public class TestSchoolExamDataContextInitService : ISchoolExamDataContextInitService
{
    public Task Init()
    {
        return Task.CompletedTask;
    }
}