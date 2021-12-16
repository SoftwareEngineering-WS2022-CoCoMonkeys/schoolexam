using System;
using System.Collections.Generic;
using System.Linq;
using SchoolExam.Core.Domain.CourseAggregate;
using SchoolExam.Core.Domain.ValueObjects;
using SchoolExam.Core.UserManagement.UserAggregate;

namespace SchoolExam.Core.Domain.PersonAggregate
{
    public class Student : Person
    {
        public static string CoursesName => nameof(_courses);
        private readonly ICollection<Course> _courses;
        public IEnumerable<Guid> CourseIds => _courses.Select(x => x.Id);

        public static string LegalGuardiansName => nameof(_legalGuardians);
        private readonly ICollection<LegalGuardian> _legalGuardians;
        public IEnumerable<Guid> LegalGuardianIds => _legalGuardians.Select(x => x.Id);
        public Guid SchoolId { get; set; }

        protected Student(Guid id) : base(id)
        {
        }

        public Student(Guid id, string firstName, string lastName, DateTime dateOfBirth, Address address,
            string emailAddress, Guid schoolId) : base(id, firstName, lastName, dateOfBirth, address, emailAddress)
        {
            SchoolId = schoolId;
            _courses = new List<Course>();
            _legalGuardians = new List<LegalGuardian>();
        }

        public override string GetRole() => Roles.Student;
    }
}