namespace SchoolExam.Util.EFAbstractions
{
    public interface IDbConnectionConfiguration
    {
        string ConnectionString { get; }
        DbTechnology Technology { get; }
    }
}