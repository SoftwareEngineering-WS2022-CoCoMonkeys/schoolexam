using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.CourseAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class CourseTeacherByTeacherSpecification : EntitySpecification<CourseTeacher>
{
    public CourseTeacherByTeacherSpecification(Guid teacherId) : base(x =>
        x.TeacherId.Equals(teacherId))
    {
        AddInclude(x => x.Course);
        AddInclude($"{nameof(CourseTeacher.Course)}.{nameof(Course.Students)}");
        AddInclude($"{nameof(CourseTeacher.Course)}.{nameof(Course.Students)}.{nameof(CourseStudent.Student)}");
    }
}