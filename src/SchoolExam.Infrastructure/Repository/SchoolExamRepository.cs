using SchoolExam.Application.Repository;
using SchoolExam.Persistence.Base;
using SchoolExam.Persistence.DataContext;

namespace SchoolExam.Infrastructure.Repository;

public class SchoolExamRepository : RepositoryBase<SchoolExamDbContext>, ISchoolExamRepository
{
    public SchoolExamRepository(SchoolExamDbContext context) : base(context)
    {
    }
}