using SchoolExam.Persistence.Base;

namespace SchoolExam.Persistence.DataContext;

public class DesignTimeDbConnectionConfiguration : IDbConnectionConfiguration
{
    public string ConnectionString =>
        "Host=localhost;Port=5433;Database=school_exam;Username=postgres;Password=postgres";
    public DbTechnology Technology => DbTechnology.PostgresSql;
}