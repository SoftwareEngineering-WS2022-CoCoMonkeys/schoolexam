namespace SchoolExam.Util
{
    public class EntityBase<TIdentity> : IEntity<TIdentity>
    {
        protected EntityBase(TIdentity id)
        {
            Id = id;
        }

        public TIdentity Id { get; }
    }
}