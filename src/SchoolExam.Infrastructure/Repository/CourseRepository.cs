using System.Collections.Generic;
using System.Linq;
using SchoolExam.Core.Domain.CourseAggregate;
using SchoolExam.Infrastructure.DataContext;

namespace SchoolExam.Infrastructure.Repository
{
    public class CourseRepository : ICourseRepository
    {
        private readonly SchoolExamDataContext _context;

        public CourseRepository(SchoolExamDataContext context)
        {
            _context = context;
        }
        
        public IEnumerable<Course> GetAll()
        {
            return _context.Courses.AsEnumerable();
        }
    }
}