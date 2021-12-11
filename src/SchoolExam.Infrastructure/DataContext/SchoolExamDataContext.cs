using System.Linq;
using System.Threading.Tasks;
using Common.Infrastructure.EFAbstractions;
using Microsoft.EntityFrameworkCore;
using SchoolExam.Core.Domain.CourseAggregate;
using SchoolExam.Core.Domain.ExamAggregate;
using SchoolExam.Core.Domain.PersonAggregate;
using SchoolExam.Core.Domain.SchoolAggregate;
using SchoolExam.Core.UserManagement.UserAggregate;
using Task = SchoolExam.Core.Domain.TaskAggregate.Task;

namespace SchoolExam.Infrastructure.DataContext
{
    public class SchoolExamDataContext : DataContextBase<SchoolExamDbContext>
    {
        public IQueryable<Course> Courses => Context.Courses.Include(Course.StudentsName);
        public IQueryable<Exam> Exams => Context.Exams.Include(x => x.GradingTable).Include(x => x.Tasks)
            .Include(x => x.Booklets).ThenInclude(x => x.Pages);
        public IQueryable<Student> Students =>
            Context.Students.Include(Student.CoursesName).Include(Student.LegalGuardiansName);
        public IQueryable<Teacher> Teachers =>
            Context.Teachers.Include(Teacher.CoursesName).Include(Teacher.SchoolsName);
        public IQueryable<LegalGuardian> LegalGuardians => Context.LegalGuardians.Include(LegalGuardian.ChildrenName);
        public IQueryable<School> Schools => Context.Schools.Include(School.TeachersName);
        public IQueryable<Task> Tasks => Context.Tasks;
        public IQueryable<User> Users => Context.Users;

        public SchoolExamDataContext(SchoolExamDbContext context) : base(context)
        {
        }
    }
}