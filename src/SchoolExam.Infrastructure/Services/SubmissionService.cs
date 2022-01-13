using SchoolExam.Application.Repository;
using SchoolExam.Application.Services;
using SchoolExam.Domain.Entities.SubmissionAggregate;
using SchoolExam.Infrastructure.Specifications;

namespace SchoolExam.Infrastructure.Services;

public class SubmissionService : ISubmissionService
{
    private readonly ISchoolExamRepository _repository;

    public SubmissionService(ISchoolExamRepository repository)
    {
        _repository = repository;
    }

    public Submission? GetById(Guid submissionId)
    {
        var result = _repository.Find(new SubmissionWithBookletWithExamByIdSpecification(submissionId));
        return result;
    }

    public IEnumerable<Submission> GetByExam(Guid examId)
    {
        var result = _repository.List(new SubmissionWithStudentAndAnswersWithTaskAndSegmentByExamSpecification(examId));
        return result;
    }

    public byte[] GetSubmissionPdf(Guid submissionId)
    {
        var pdfFile = _repository.Find(new SubmissionPdfFileBySubmissionSpecification(submissionId));
        if (pdfFile == null)
        {
            throw new ArgumentException("There exists no PDF file for the submission.");
        }
        
        return pdfFile.Content;
    }
}