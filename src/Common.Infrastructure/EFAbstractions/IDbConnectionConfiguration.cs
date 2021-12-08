namespace Common.Infrastructure.EFAbstractions
{
    public interface IDbConnectionConfiguration
    {
        string ConnectionString { get; }
        DbTechnology Technology { get; }
    }
}