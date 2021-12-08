using System.Collections.Generic;
using System.Linq;

namespace SchoolExam.Core.Domain.CourseAggregate
{
    public interface ICourseRepository
    {
        IEnumerable<Course> GetAll();
    }
}