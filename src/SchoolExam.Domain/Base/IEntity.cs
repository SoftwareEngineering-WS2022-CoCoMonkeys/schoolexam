namespace SchoolExam.Domain.Base
{
    public interface IEntity<TIdentity>
    {
        TIdentity Id { get; set; }
    }
}