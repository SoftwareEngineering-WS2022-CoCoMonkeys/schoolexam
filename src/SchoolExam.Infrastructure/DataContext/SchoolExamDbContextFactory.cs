using System;
using Common.Infrastructure.EFAbstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SchoolExam.Infrastructure.DataContext
{
    public class SchoolExamDbContextFactory : IDesignTimeDbContextFactory<SchoolExamDbContext>
    {
        private readonly IDbConnectionConfiguration _configuration;
        
        public SchoolExamDbContextFactory()
        {
            _configuration = new DesignTimeDbConnectionConfiguration();
        }
        
        public SchoolExamDbContext CreateDbContext(string[] args)
        {
            return new SchoolExamDbContext(_configuration);
        }
    }
}