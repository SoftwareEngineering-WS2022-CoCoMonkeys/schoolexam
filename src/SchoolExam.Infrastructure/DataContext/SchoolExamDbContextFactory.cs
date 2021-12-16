using Microsoft.EntityFrameworkCore.Design;
using SchoolExam.Application.Authentication;
using SchoolExam.Infrastructure.Authentication;
using SchoolExam.Util.EFAbstractions;

namespace SchoolExam.Infrastructure.DataContext
{
    public class SchoolExamDbContextFactory : IDesignTimeDbContextFactory<SchoolExamDbContext>
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IDbConnectionConfiguration _configuration;
        
        public SchoolExamDbContextFactory()
        {
            _configuration = new DesignTimeDbConnectionConfiguration();
            _passwordHasher = new BCryptPasswordHasher();
        }
        
        public SchoolExamDbContext CreateDbContext(string[] args)
        {
            return new SchoolExamDbContext(_configuration, _passwordHasher);
        }
    }
}