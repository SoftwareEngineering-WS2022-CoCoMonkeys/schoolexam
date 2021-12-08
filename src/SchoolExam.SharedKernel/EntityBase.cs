using System;

namespace SchoolExam.SharedKernel
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