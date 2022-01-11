using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.Entities.PersonAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class StudentByExamSpecification : EntitySpecification<Student>
{
    public StudentByExamSpecification(Guid examId) : base(x =>
        x.Courses.SelectMany(c => c.Course.Exams).Any(e => e.Id.Equals(examId)))
    {
        AddInclude(x => x.Courses);
        AddInclude($"{nameof(Student.Courses)}.{nameof(CourseStudent.Course)}");
        AddInclude($"{nameof(Student.Courses)}.{nameof(CourseStudent.Course)}.{nameof(Course.Exams)}");
    }
}