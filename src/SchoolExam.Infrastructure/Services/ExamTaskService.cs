using System.Text.RegularExpressions;
using SchoolExam.Application.Pdf;
using SchoolExam.Application.Repository;
using SchoolExam.Application.Services;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Exceptions;
using SchoolExam.Domain.Extensions;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Infrastructure.Specifications;

namespace SchoolExam.Infrastructure.Services;

public class ExamTaskService : ExamServiceBase, IExamTaskService
{
    private static string _guidRegex = "[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}";

    private readonly IPdfService _pdfService;
    
    public ExamTaskService(ISchoolExamRepository repository, IPdfService pdfService) : base(repository)
    {
        _pdfService = pdfService;
    }

    public async Task SetTaskPdfFile(Guid examId, Guid userId, byte[] content, params ExamTaskInfo[] tasks)
    {
        var exam = EnsureExamExists(new ExamWithTaskPdfFileAndGradingTableByIdSpecification(examId));

        if (exam.State.HasBeenBuilt())
        {
            throw new DomainException("The task PDF file of an exam that already has been built cannot be changed.");
        }

        // remove previously existing task PDF file
        if (exam.TaskPdfFile != null)
        {
            Repository.Remove(exam.TaskPdfFile);
        }

        // remove previously existing grading table since points might not be valid anymore
        if (exam.GradingTable != null)
        {
            Repository.Remove(exam.GradingTable);
        }

        // make sure that the exam has at least one task
        if (tasks.Length < 1)
        {
            throw new DomainException("An exam must contain at least one task.");
        }

        var taskPdfFile =
            new TaskPdfFile(Guid.NewGuid(), $"{examId.ToString()}.pdf", content.LongLength, DateTime.Now, userId,
                content, examId);
        // exam is ready to be built after having a task PDF file
        exam.State = ExamState.BuildReady;
        Repository.Update(exam);
        Repository.Add(taskPdfFile);

        FindTasks(examId, userId, content, tasks);

        await Repository.SaveChangesAsync();
    }

    private void FindTasks(Guid examId, Guid userId, byte[] taskPdf, params ExamTaskInfo[] tasks)
    {
        var exam = EnsureExamExists(new ExamWithTaskPdfFileAndTasksByIdSpecification(examId));

        // reset existing exam tasks
        var currentExamTasks = exam.Tasks;
        foreach (var task in currentExamTasks)
        {
            Repository.Remove(task);
        }

        // check that maximum points of all tasks are positive
        foreach (var task in tasks)
        {
            if (task.MaxPoints <= 0.0)
            {
                throw new DomainException("Maximum number of points must be a positive number.");
            }
        }

        var pdf = taskPdf;
        var links = _pdfService.GetUriLinkAnnotations(pdf).ToList();
        var tasksDict = tasks.ToDictionary(x => x.Id, x => x);

        var startLinkCandidates = links.Where(x => Regex.IsMatch(x.Uri, $"^task-start-{_guidRegex}$")).ToList();
        var endLinkCandidates = links.Where(x => Regex.IsMatch(x.Uri, $"^task-end-{_guidRegex}$")).ToList();
        var linkCandidates = startLinkCandidates.Union(endLinkCandidates).ToList();
        // check if there are duplicate markers
        if (linkCandidates.Select(x => x.Uri).Distinct().Count() != linkCandidates.Count)
        {
            var task = linkCandidates.GroupBy(x => x.Uri).First(x => x.Count() > 1);
            throw new DomainException($"Task marker with text {task.Key} was found in PDF more than once.");
        }

        var endLinkCandidatesDict = endLinkCandidates.ToDictionary(x => x.Uri, x => x);

        var matchedLinks = new List<PdfUriLinkAnnotationInfo>();
        var matchedTaskIds = new HashSet<Guid>();
        // iterate through detected start markers
        foreach (var startLink in startLinkCandidates)
        {
            // extract task identifier
            var taskIdString = Regex.Match(startLink.Uri, $"{_guidRegex}$").Value;
            var taskId = Guid.Parse(taskIdString);
            // check if the exam has a task with the extracted identifier
            if (tasksDict.ContainsKey(taskId))
            {
                // find corresponding end marker
                var taskEndString = $"task-end-{taskId}";
                if (!endLinkCandidatesDict.ContainsKey(taskEndString))
                {
                    throw new DomainException($"No end marker was found for task with id {taskId}.");
                }

                var endLink = endLinkCandidatesDict[taskEndString];

                matchedTaskIds.Add(taskId);
                var task = tasksDict[taskId];
                // add start and end marker to list such that they can be removed from the PDF file afterwards
                matchedLinks.Add(startLink);
                matchedLinks.Add(endLink);
                var examTask = new ExamTask(Guid.NewGuid(), task.Title, task.MaxPoints, examId,
                    new ExamPosition(startLink.Page, startLink.Top), new ExamPosition(endLink.Page, endLink.Bottom));
                Repository.Add(examTask);
            }
        }

        if (matchedTaskIds.Count != tasks.Length)
        {
            var unmatchedTask = tasks.First(x => !matchedTaskIds.Contains(x.Id));
            throw new DomainException($"Task with id {unmatchedTask.Id} could not be found in PDF.");
        }

        var pdfWithoutTaskLinks = _pdfService.RemoveUriLinkAnnotations(pdf, matchedLinks.ToArray());
        Repository.Remove(exam.TaskPdfFile!);
        var newTaskPdfFile = new TaskPdfFile(Guid.NewGuid(), $"{examId}.pdf", pdfWithoutTaskLinks.LongLength,
            DateTime.Now, userId, pdfWithoutTaskLinks, examId);
        Repository.Add(newTaskPdfFile);
        Repository.Update(exam);
    }
}