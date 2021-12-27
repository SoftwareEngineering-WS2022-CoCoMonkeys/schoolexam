using SchoolExam.Application.DataContext;
using SchoolExam.Application.Repositories;
using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.Infrastructure.Repositories;

public class ExamRepository : IExamRepository
{
    private readonly ISchoolExamDataContext _context;

    public ExamRepository(ISchoolExamDataContext context)
    {
        _context = context;
    }

    public async Task SetTaskPdfFile(Guid examId, string name, Guid uploaderId, byte[] content)
    {
        var exam = _context.Find<Exam, Guid>(examId);
        if (exam == null)
        {
            throw new ArgumentException("Exam does not exist");
        }

        var taskPdfFile = new TaskPdfFile(Guid.NewGuid(), name, content.LongLength, DateTime.Now, uploaderId, content,
            examId);
        _context.Add(taskPdfFile);
        await _context.SaveChangesAsync();
    }
}