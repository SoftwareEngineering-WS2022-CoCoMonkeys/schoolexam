namespace SchoolExam.Domain.Base
{
    public class EntityBase : IEntity
    {
        protected EntityBase(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}