using SchoolExam.Application.Publishing;
using SchoolExam.Application.Repository;
using SchoolExam.Application.Services;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Exceptions;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Infrastructure.Specifications;

namespace SchoolExam.Infrastructure.Services;

public class ExamPublishService : ExamServiceBase, IExamPublishService
{
    private readonly IPublishingService _publishingService;

    public ExamPublishService(ISchoolExamRepository repository, IPublishingService publishingService) : base(repository)
    {
        _publishingService = publishingService;
    }

    public async Task Publish(Guid examId, DateTime? publishDateTime)
    {
        var exam = EnsureExamExists(new ExamWithGradingTableById(examId));
        if (exam.State == ExamState.Published)
        {
            throw new DomainException("Exam is already published.");
        }

        if (exam.State != ExamState.Corrected)
        {
            throw new DomainException("Correction of exam must be completed before publishing exam.");
        }

        if (exam.GradingTable == null)
        {
            throw new DomainException("Exam does not have a grading table.");
        }

        if (publishDateTime.HasValue && publishDateTime.Value > DateTime.UtcNow)
        {
            var scheduledExamId = Guid.NewGuid();
            var scheduledExam = new ScheduledExam(scheduledExamId, examId, publishDateTime.Value, false);
            Repository.Add(scheduledExam);
            await _publishingService.ScheduleSendEmailToStudent(examId, publishDateTime.Value);
        }
        else
        {
            await _publishingService.DoPublishExam(examId);
        }
    }
}