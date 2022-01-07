using Microsoft.EntityFrameworkCore;
using SchoolExam.Application.DataContext;
using SchoolExam.Persistence.DataContext;

namespace SchoolExam.Infrastructure.DataContext;

public class SchoolExamDataContextInitService : ISchoolExamDataContextInitService
{
    private readonly SchoolExamDbContext _context;
    
    public SchoolExamDataContextInitService(SchoolExamDbContext context)
    {
        _context = context;
    }
    
    public async Task Init()
    {
        await _context.Database.MigrateAsync();
    }
}