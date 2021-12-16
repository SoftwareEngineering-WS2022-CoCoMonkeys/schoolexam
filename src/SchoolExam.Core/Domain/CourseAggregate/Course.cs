using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SchoolExam.Core.Domain.PersonAggregate;
using SchoolExam.Core.Domain.ValueObjects;
using SchoolExam.Util;

namespace SchoolExam.Core.Domain.CourseAggregate
{
    public class Course : EntityBase<Guid>
    {
        public Subject? Subject { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Year { get; set; }
        public Guid SchoolId { get; set; }

        public static string StudentsName => nameof(_students);
        private readonly ICollection<Student> _students;
        public IEnumerable<Guid> StudentIds => _students.Select(x => x.Id);

        protected Course(Guid id) : base(id)
        {
        }

        public Course(Guid id, string name, string description, Subject subject, int year, Guid schoolId) : this(id)
        {
            Name = name;
            Description = description;
            Subject = subject;
            Year = year;
            SchoolId = schoolId;
            _students = new List<Student>();
        }
    }
}