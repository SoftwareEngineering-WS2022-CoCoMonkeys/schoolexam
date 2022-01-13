using Microsoft.EntityFrameworkCore;
using SchoolExam.Application.Repository;
using SchoolExam.Persistence.DataContext;

namespace SchoolExam.Infrastructure.Repository;

public class SchoolExamRepositoryInitService : ISchoolExamRepositoryInitService
{
    private readonly SchoolExamDbContext _context;
    
    public SchoolExamRepositoryInitService(SchoolExamDbContext context)
    {
        _context = context;
    }
    
    public async Task Init()
    {
        await _context.Database.MigrateAsync();
    }
}