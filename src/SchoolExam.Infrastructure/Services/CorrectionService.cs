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

public class CorrectionService : ExamServiceBase, ICorrectionService
{
    public CorrectionService(ISchoolExamRepository repository) : base(repository)
    {
        
    }
    
    public double GetMaxPoints(Guid examId)
    {
        var exam = EnsureExamExists(new ExamWithTasksByIdSpecification(examId));
        var maxPoints = exam.Tasks.Sum(x => x.MaxPoints);
        return maxPoints;
    }

    public async Task SetGradingTable(Guid examId, params GradingTableIntervalLowerBound[] lowerBounds)
    {
        var exam = EnsureExamExists(new ExamWithGradingTableById(examId));
        if (exam.State is ExamState.Planned)
        {
            throw new DomainException("An exam without a task PDF file cannot have a grading table.");
        }

        // remove previously existing grading table
        if (exam.GradingTable != null)
        {
            Repository.Remove(exam.GradingTable);
        }

        var tasks = Repository.List(new ExamTaskByExamSpecification(examId));
        var maxPoints = tasks.Sum(x => x.MaxPoints);

        var lowerBoundsOrdered = lowerBounds.OrderBy(x => x.Points).ToArray();
        var count = lowerBoundsOrdered.Length;
        if (count < 1)
        {
            throw new DomainException("Grading table must contain at least one interval.");
        }

        if (lowerBoundsOrdered[0].Points != 0.0)
        {
            throw new DomainException("A grading interval starting from 0.0 points must be included.");
        }

        var gradingTableId = Guid.NewGuid();
        for (int i = 0; i < count; i++)
        {
            var current = lowerBoundsOrdered[i];
            if (current.Points > maxPoints)
            {
                throw new DomainException("A grading interval exceeds the maximum number of points.");
            }
            
            if (i == count - 1)
            {
                break;
            }

            var next = lowerBoundsOrdered[i + 1];
            if (current.Points == next.Points)
            {
                throw new DomainException("A grading interval must not be empty.");
            }

            var lowerBound = new GradingTableIntervalBound(current.Points, GradingTableIntervalBoundType.Inclusive);
            var upperBound = new GradingTableIntervalBound(next.Points, GradingTableIntervalBoundType.Exclusive);
            Repository.Add(new GradingTableInterval(lowerBound, upperBound, current.Grade, current.Type,
                gradingTableId));
        }

        // deal with last bound separately
        var last = lowerBoundsOrdered[count - 1];
        var lowerBoundLast = new GradingTableIntervalBound(last.Points, GradingTableIntervalBoundType.Inclusive);
        var upperBoundLast = new GradingTableIntervalBound(maxPoints, GradingTableIntervalBoundType.Inclusive);
        Repository.Add(new GradingTableInterval(lowerBoundLast, upperBoundLast, last.Grade, last.Type,
            gradingTableId));

        var gradingTable = new GradingTable(gradingTableId, examId);
        Repository.Add(gradingTable);

        await Repository.SaveChangesAsync();
    }
    
    public byte[] GetSubmissionPdf(Guid submissionId)
    {
        var pdfFile = Repository.Find(new SubmissionPdfFileBySubmissionSpecification(submissionId));
        if (pdfFile == null)
        {
            throw new DomainException("There exists no PDF file for the submission.");
        }

        return pdfFile.Content;
    }
    
    public byte[] GetRemarkPdf(Guid submissionId)
    {
        var pdfFile = Repository.Find(new RemarkPdfFileBySubmissionSpecification(submissionId));
        if (pdfFile == null)
        {
            throw new DomainException("There exists no PDF file with remarks for the submission.");
        }

        return pdfFile.Content;
    }

    public async Task SetPoints(Guid submissionId, Guid taskId, double? points)
    {
        var answer = Repository.Find(new AnswerBySubmissionAndTaskSpecification(submissionId, taskId));
        if (answer == null)
        {
            throw new DomainException(
                $"There exists no submission with identifier {submissionId} that contains a task with identifier {taskId}.");
        }

        var task = Repository.Find<ExamTask>(taskId)!;

        if (points < 0.0 || points > task.MaxPoints)
        {
            throw new DomainException("Points are not in allowed range between 0 and maximum points.");
        }

        answer.AchievedPoints = points;
        answer.State = answer.AchievedPoints.HasValue ? AnswerState.Corrected : AnswerState.Pending;
        answer.UpdatedAt = DateTime.Now.SetKindUtc();
        Repository.Update(answer);

        var submission = Repository.Find(new SubmissionWithBookletByIdSpecification(answer.SubmissionId))!;
        var examId = submission.Booklet.ExamId;

        var submissions = Repository.List(new SubmissionWithAnswersByExamSpecification(examId));
        var exam = Repository.Find<Exam>(examId)!;
        exam.State = submissions.All(x => x.GetCorrectionState() == CorrectionState.Corrected)
            ? ExamState.Corrected
            : ExamState.InCorrection;
        Repository.Update(exam);

        await Repository.SaveChangesAsync();
    }

    public async Task SetRemark(Guid submissionId, byte[] remarkPdf, Guid userId)
    {
        var submission = Repository.Find(new SubmissionWithRemarkPdfByIdSpecification(submissionId));
        if (submission == null)
        {
            throw new DomainException("Submission does not exist.");
        }

        if (submission.RemarkPdfFile != null)
        {
            Repository.Remove(submission.RemarkPdfFile);
        }

        var remarkPdfFile = new RemarkPdfFile(Guid.NewGuid(), $"{submission.Id}_corrected.pdf",
            remarkPdf.LongLength, DateTime.Now.SetKindUtc(), userId, remarkPdf, submissionId);
        Repository.Add(remarkPdfFile);

        await Repository.SaveChangesAsync();
    }
}