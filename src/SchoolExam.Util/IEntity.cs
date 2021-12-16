namespace SchoolExam.Util
{
    public interface IEntity<out TIdentity>
    {
        TIdentity Id { get; }
    }
}