using SchoolExam.Application.Repository;
using SchoolExam.Application.Services;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;
using SchoolExam.Domain.Exceptions;
using SchoolExam.Domain.Extensions;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Extensions;
using SchoolExam.Infrastructure.Extensions;
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

    public IEnumerable<Submission> GetByIds(IEnumerable<Guid> ids)
    {
        var result = _repository.List(new SubmissionWithBookletWithExamByIdsSpecification(ids.ToHashSet()));
        return result;
    }

    public Submission? GetByIdWithDetails(Guid submissionId)
    {
        var result =
            _repository.Find(
                new SubmissionWithPdfFileAndStudentAndAnswersWithTaskAndSegmentsByIdSpecification(submissionId));
        return result;
    }

    public IEnumerable<Submission> GetByIdsWithDetails(IEnumerable<Guid> ids)
    {
        var result =
            _repository.List(
                new SubmissionWithPdfFileAndStudentAndAnswersWithTaskAndSegmentsByIdsSpecification(ids.ToHashSet()));
        return result;
    }

    public IEnumerable<Submission> GetByExam(Guid examId)
    {
        var result = _repository.List(new SubmissionWithPdfFileAndStudentAndAnswersByExamSpecification(examId));
        return result;
    }
}