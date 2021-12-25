using SchoolExam.Persistence.Base;

namespace SchoolExam.Persistence.DataContext;

public class DbConnectionConfiguration : IDbConnectionConfiguration
{
    public string ConnectionString =>
        "Host=postgres-develop;Port=5432;Database=school_exam;Username=postgres;Password=postgres";
    public DbTechnology Technology => DbTechnology.PostgresSql;
}