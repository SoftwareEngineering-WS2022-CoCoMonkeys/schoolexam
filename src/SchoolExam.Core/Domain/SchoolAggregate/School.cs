using System;
using System.Collections.Generic;
using System.Linq;
using SchoolExam.Core.Domain.PersonAggregate;
using SchoolExam.Core.Domain.ValueObjects;
using SchoolExam.Util;

namespace SchoolExam.Core.Domain.SchoolAggregate
{
    public class School : EntityBase<Guid>
    {
        public string Name { get; set; }
        public Address Location { get; set; }
        public bool HasScanner { get; set; }

        public static string TeachersName = nameof(_teachers);
        private readonly ICollection<Teacher> _teachers;
        public IEnumerable<Guid> TeacherIds => _teachers.Select(x => x.Id);

        protected School(Guid id) : base(id) 
        {
        }
        
        public School(Guid id, string name, Address location, bool hasScanner) : this(id)
        {
            Name = name;
            Location = location;
            HasScanner = hasScanner;
            _teachers = new List<Teacher>();
        }
    }
}