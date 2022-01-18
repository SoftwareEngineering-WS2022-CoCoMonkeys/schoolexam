using Microsoft.EntityFrameworkCore;
using SchoolExam.Application.Repository;
using SchoolExam.Persistence.DataContext;

namespace SchoolExam.Infrastructure.Repository;

public class ProductionSchoolExamRepositoryInitService : ISchoolExamRepositoryInitService
{
    private readonly SchoolExamDbContext _context;
    
    public ProductionSchoolExamRepositoryInitService(SchoolExamDbContext context)
    {
        _context = context;
    }
    
    public async Task Init()
    {
        await _context.Database.MigrateAsync();
    }
}