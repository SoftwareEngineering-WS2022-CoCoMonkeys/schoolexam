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

    public byte[] GetSubmissionPdf(Guid submissionId)
    {
        var pdfFile = _repository.Find(new SubmissionPdfFileBySubmissionSpecification(submissionId));
        if (pdfFile == null)
        {
            throw new DomainException("There exists no PDF file for the submission.");
        }

        return pdfFile.Content;
    }
    
    public byte[] GetRemarkPdf(Guid submissionId)
    {
        var pdfFile = _repository.Find(new RemarkPdfFileBySubmissionSpecification(submissionId));
        if (pdfFile == null)
        {
            throw new DomainException("There exists no PDF file with remarks for the submission.");
        }

        return pdfFile.Content;
    }

    public async Task SetPoints(Guid submissionId, Guid taskId, double? points)
    {
        var answer = _repository.Find(new AnswerBySubmissionAndTaskSpecification(submissionId, taskId));
        if (answer == null)
        {
            throw new DomainException(
                $"There exists no submission with identifier {submissionId} that contains a task with identifier {taskId}");
        }

        var task = _repository.Find<ExamTask>(taskId)!;

        if (points < 0.0 || points > task.MaxPoints)
        {
            throw new DomainException("Points are not in allowed range between 0 and maximum points.");
        }

        answer.AchievedPoints = points;
        answer.State = answer.AchievedPoints.HasValue ? AnswerState.Corrected : AnswerState.Pending;
        answer.UpdatedAt = DateTime.Now.SetKindUtc();
        _repository.Update(answer);

        var submission = _repository.Find(new SubmissionWithBookletByIdSpecification(answer.SubmissionId))!;
        var examId = submission.Booklet.ExamId;

        var submissions = _repository.List(new SubmissionWithAnswersByExamSpecification(examId));
        var exam = _repository.Find<Exam>(examId)!;
        exam.State = submissions.All(x => x.GetCorrectionState() == CorrectionState.Corrected)
            ? ExamState.Corrected
            : ExamState.InCorrection;
        _repository.Update(exam);

        await _repository.SaveChangesAsync();
    }

    public async Task SetRemark(Guid submissionId, byte[] remarkPdf, Guid userId)
    {
        var submission = _repository.Find(new SubmissionWithRemarkPdfByIdSpecification(submissionId));
        if (submission == null)
        {
            throw new DomainException("Submission does not exist.");
        }

        if (submission.RemarkPdfFile != null)
        {
            _repository.Remove(submission.RemarkPdfFile);
        }

        var remarkPdfFile = new RemarkPdfFile(Guid.NewGuid(), $"{submission.Id}_corrected.pdf",
            remarkPdf.LongLength, DateTime.Now.SetKindUtc(), userId, remarkPdf, submissionId);
        _repository.Add(remarkPdfFile);

        await _repository.SaveChangesAsync();
    }
}