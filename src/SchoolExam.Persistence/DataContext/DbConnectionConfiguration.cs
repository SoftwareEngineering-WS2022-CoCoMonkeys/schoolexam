using SchoolExam.Persistence.Base;

namespace SchoolExam.Persistence.DataContext;

public class DbConnectionConfiguration : IDbConnectionConfiguration
{
    public string ConnectionString { get; }
    public DbTechnology Technology => DbTechnology.PostgresSql;

    public DbConnectionConfiguration(string connectionString)
    {
        ConnectionString = connectionString;
    }
}