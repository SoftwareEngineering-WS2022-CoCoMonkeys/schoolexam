using SchoolExam.Application.Repository;
using SchoolExam.Application.Services;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;
using SchoolExam.Domain.ValueObjects;
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

    public byte[] GetSubmissionPdf(Guid submissionId)
    {
        var pdfFile = _repository.Find(new SubmissionPdfFileBySubmissionSpecification(submissionId));
        if (pdfFile == null)
        {
            throw new ArgumentException("There exists no PDF file for the submission.");
        }

        return pdfFile.Content;
    }

    public async Task SetPoints(Guid submissionId, Guid taskId, double? points)
    {
        var answer = _repository.Find(new AnswerBySubmissionAndTaskSpecification(submissionId, taskId));
        if (answer == null)
        {
            throw new ArgumentException(
                $"There exists no submission with identifier {submissionId} that contains a task with identifier {taskId}");
        }

        var task = _repository.Find<ExamTask, Guid>(taskId)!;

        if (points < 0.0 || points > task.MaxPoints)
        {
            throw new ArgumentException("Points are not in allowed range between 0 and maximum points.");
        }

        answer.AchievedPoints = points;
        answer.State = answer.AchievedPoints.HasValue ? AnswerState.Corrected : AnswerState.Pending;
        _repository.Update(answer);
        await _repository.SaveChangesAsync();
    }
}