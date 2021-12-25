using Microsoft.EntityFrameworkCore;

namespace SchoolExam.Persistence.Base;

public abstract class DbContextBase : DbContext
{
    private readonly IDbConnectionConfiguration _configuration;
        
    public DbContextBase(IDbConnectionConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        switch (_configuration.Technology)
        {
            case DbTechnology.PostgresSql:
                optionsBuilder.UseNpgsql(_configuration.ConnectionString);
                break;
            case DbTechnology.EntityFrameworkInMemory:
                optionsBuilder.UseInMemoryDatabase(_configuration.ConnectionString);
                break;
            default:
                throw new ArgumentException("Configured database technology is not supported.");
        }
        base.OnConfiguring(optionsBuilder);
    }
}