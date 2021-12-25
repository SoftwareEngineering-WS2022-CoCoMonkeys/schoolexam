namespace SchoolExam.Domain.Base
{
    public class EntityBase<TIdentity> : IEntity<TIdentity>
    {
        protected EntityBase(TIdentity id)
        {
            Id = id;
        }

        public TIdentity Id { get; set; }
    }
}