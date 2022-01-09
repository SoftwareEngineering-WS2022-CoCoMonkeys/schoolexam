using SchoolExam.Application.DataContext;
using SchoolExam.Persistence.Base;
using SchoolExam.Persistence.DataContext;

namespace SchoolExam.Infrastructure.DataContext;

public class SchoolExamDataContext : DataContextBase<SchoolExamDbContext>, ISchoolExamDataContext
{
    public SchoolExamDataContext(SchoolExamDbContext context) : base(context)
    {
    }
}