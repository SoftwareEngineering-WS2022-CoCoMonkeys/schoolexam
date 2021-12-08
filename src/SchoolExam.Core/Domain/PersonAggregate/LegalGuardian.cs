using System;
using System.Collections.Generic;
using System.Linq;
using SchoolExam.Core.Domain.ValueObjects;

namespace SchoolExam.Core.Domain.PersonAggregate
{
    public class LegalGuardian : Person
    {
        public static string ChildrenName = nameof(_children);
        private readonly ICollection<Student> _children;
        public IEnumerable<Guid> ChildIds => _children.Select(x => x.Id);

        protected LegalGuardian(Guid id) : base(id)
        {
        }

        public LegalGuardian(Guid id, string firstName, string lastName, DateTime dateOfBirth, Address address) : base(id, firstName, lastName, dateOfBirth, address)
        {
            _children = new List<Student>();
        }
    }
}