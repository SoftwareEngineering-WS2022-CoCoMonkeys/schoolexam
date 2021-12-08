namespace SchoolExam.SharedKernel
{
    public interface IEntity<out TIdentity>
    {
        TIdentity Id { get; }
    }
}