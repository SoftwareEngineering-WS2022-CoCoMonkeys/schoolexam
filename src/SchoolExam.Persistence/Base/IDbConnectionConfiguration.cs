namespace SchoolExam.Persistence.Base;

public interface IDbConnectionConfiguration
{
    string ConnectionString { get; }
    DbTechnology Technology { get; }
}