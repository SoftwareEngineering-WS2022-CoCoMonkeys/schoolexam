using System;
using System.Collections.Generic;
using System.Linq;
using SchoolExam.Core.Domain.CourseAggregate;
using SchoolExam.Core.Domain.SchoolAggregate;
using SchoolExam.Core.Domain.ValueObjects;

namespace SchoolExam.Core.Domain.PersonAggregate
{
    public class Teacher : Person
    {
        public static string CoursesName = nameof(_courses);
        private readonly ICollection<Course> _courses;
        public IEnumerable<Guid> CourseIds => _courses.Select(x => x.Id);

        public static string SchoolsName = nameof(_schools);
        private readonly ICollection<School> _schools;
        public IEnumerable<Guid> SchoolIds => _schools.Select(x => x.Id);

        protected Teacher(Guid id) : base(id)
        {
        }

        public Teacher(Guid id, string firstName, string lastName, DateTime dateOfBirth, Address address) : base(id,
            firstName, lastName, dateOfBirth, address)
        {
            _courses = new List<Course>();
            _schools = new List<School>();
        }
    }
}