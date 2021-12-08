using Common.Infrastructure.EFAbstractions;

namespace SchoolExam.Infrastructure.DataContext
{
    public class DbConnectionConfiguration : IDbConnectionConfiguration
    {
        public string ConnectionString =>
            "Host=postgres-develop;Port=5432;Database=school_exam;Username=postgres;Password=postgres";
        public DbTechnology Technology => DbTechnology.PostgresSql;
    }
}