using Microsoft.EntityFrameworkCore.Design;
using SchoolExam.Persistence.Base;

namespace SchoolExam.Persistence.DataContext;

public class SchoolExamDbContextFactory : IDesignTimeDbContextFactory<SchoolExamDbContext>
{
    private readonly IDbConnectionConfiguration _configuration;
        
    public SchoolExamDbContextFactory()
    {
        _configuration = new DesignTimeDbConnectionConfiguration(); ;
    }
        
    public SchoolExamDbContext CreateDbContext(string[] args)
    {
        return new SchoolExamDbContext(_configuration);
    }
}