using SchoolExam.Application.QrCode;
using SchoolExam.Application.Repository;
using SchoolExam.Application.Services;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Infrastructure.Extensions;

namespace SchoolExam.Infrastructure.Services;

public class StudentService : IStudentService
{
    private readonly ISchoolExamRepository _repository;
    private readonly IQrCodeGenerator _qrCodeGenerator;
    
    public StudentService(ISchoolExamRepository repository, IQrCodeGenerator qrCodeGenerator)
    {
        _repository = repository;
        _qrCodeGenerator = qrCodeGenerator;
    }
    
    public byte[] GetQrCodeByStudentId(Guid studentId)
    {
        var student = _repository.Find<Student, Guid>(studentId);
        if (student == null)
        {
            throw new ArgumentException("Student does not exist");
        }

        var result = _qrCodeGenerator.GeneratePngQrCode(student.QrCode.Data);
        return result;
    }
}