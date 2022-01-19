using System.Text;
using MailKit.Net.Smtp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MimeKit;
using SchoolExam.Application.Pdf;
using SchoolExam.Application.Publishing;
using SchoolExam.Application.Repository;
using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Infrastructure.Extensions;
using SchoolExam.Infrastructure.Specifications;

namespace SchoolExam.Infrastructure.Publishing;

public class PublishingService : IPublishingService
{
    private ISchoolExamRepository _repository;
    private readonly IPdfService _pdfService;
    private Timer _timer = null!;
    private readonly ILogger<PublishingService> _logger;
    private readonly IServiceScopeFactory _factory;

    public PublishingService(ISchoolExamRepository repository, IPdfService pdfService,
        ILogger<PublishingService> logger, IServiceScopeFactory factory)
    {
        _repository = repository;
        _pdfService = pdfService;
        _logger = logger;
        _factory = factory;
    }

    private bool SendEmailToStudent(Booklet booklet, Exam exam)
    {
        var student = booklet.Submission!.Student;
        if (student == null)
        {
            return false;
        }

        var remarkPdf = booklet.Submission.RemarkPdfFile!.Content;

        // Create a message and set up the recipients.
        var mailSubject =
            $"Deine Note in {exam.Title} am {exam.Date.Day}.{exam.Date.Month}.{exam.Date.Year} {exam.Topic.Name}";
        var mailLine1 = $"Hallo {student.FirstName} {student.LastName}!";
        var mailLine2 = $"Dein Prüfungsergebnis in {exam.Title} am {exam.Date:dd.MM.yyyy} wurde gerade veröffentlicht!";
        var mailLine3 = "Du findest deine Note sowie die korrigierte Prüfung in der PDF-Datei im Anhang!";
        var mailLine4 =
            "Diese Mail wurde automatisch von SchoolExam erstellt. Bitte wende dich bei Fragen an deine Lehrkraft.";

        var messageMailKit = new MimeMessage();
        messageMailKit.From.Add(new MailboxAddress("SchoolExam", "schoolexam@rootitup.de"));
        messageMailKit.To.Add(new MailboxAddress($"{student.FirstName} {student.LastName}", student.EmailAddress));
        messageMailKit.Subject = mailSubject;

        var body = new TextPart("plain") {Text = $"{mailLine1}\n\n{mailLine2}\n{mailLine3}\n\n{mailLine4}"};

        var gradePagePdf = GenerateGradePdf(booklet.Id);
        var mergedPdf = _pdfService.Merge(gradePagePdf, remarkPdf);

        // protect PDF
        var userPassword = "schoolexam";
        var ownerPassword = "schoolexam";
        var protectedPdf = _pdfService.Protect(mergedPdf, Encoding.UTF8.GetBytes(userPassword),
            Encoding.UTF8.GetBytes(ownerPassword));

        var attachment = new MimePart("application", "pdf")
        {
            Content = new MimeContent(new MemoryStream(protectedPdf)),
            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
            ContentTransferEncoding = ContentEncoding.Base64,
            FileName = $"Prüfungskorrektur_{student.LastName}_{exam.Title}_{exam.Date:dd.MM.yyyy}.pdf"
        };
        var multipart = new Multipart("mixed");
        multipart.Add(body);
        multipart.Add(attachment);

        messageMailKit.Body = multipart;

        using SmtpClient mailKitClient = new SmtpClient();
        mailKitClient.Connect("mail01.rootitup.de", 587, false);

        // Note: since we don't have an OAuth2 token, disable the XOAUTH2 authentication mechanism.
        mailKitClient.AuthenticationMechanisms.Remove("XOAUTH2");

        mailKitClient.Authenticate("schoolexam", "DiesesPasswortIstSehrSicher12345#");

        try
        {
            mailKitClient.Send(messageMailKit);
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"Exception caught in CreateMessageWithAttachment(): {ex}");
        }

        return true;
    }

    public async Task ScheduleSendEmailToStudent(Guid examId, DateTime publishDateTime)
    {
        var scheduledExamId = Guid.NewGuid();
        var scheduledExam = new ScheduledExam(scheduledExamId, examId, publishDateTime, false);
        _repository.Add(scheduledExam);
        await _repository.SaveChangesAsync();

        DateTime current = DateTime.Now;
        TimeSpan timeToGo = publishDateTime - current;
        if (timeToGo < TimeSpan.Zero)
        {
            await DoPublishExam(examId);
        }
        else
        {
            _timer = new Timer(async x =>
            {
                using (var scope = _factory.CreateScope())
                {
                    _repository = scope.ServiceProvider.GetRequiredService<ISchoolExamRepository>();
                    await DoPublishExam(examId);
                }
            }, null, timeToGo, Timeout.InfiniteTimeSpan);
        }

        scheduledExam.IsPublished = true;
        await _repository.SaveChangesAsync();
    }

    public async Task DoPublishExam(Guid examId)
    {
        var exam = _repository.Find(new EntityByIdSpecification<Exam>(examId))!;
        var booklets = _repository.List(new BookletWithSubmissionWithStudentWithRemarkPdfByExamSpecification(exam.Id));
        foreach (Booklet booklet in booklets)
        {
            SendEmailToStudent(booklet, exam);
        }

        exam.State = ExamState.Published;
        _repository.Update(exam);
        await _repository.SaveChangesAsync();
    }

    private byte[] GenerateGradePdf(Guid bookletId)
    {
        var booklet = _repository.Find<Booklet>(bookletId)!;
        var submission = _repository.Find(new SubmissionWithAnswersByBookletSpecification(bookletId))!;
        var examId = booklet.ExamId;
        var exam = _repository.Find(new ExamWithGradingTableAndTasksById(examId))!;
        var maxPoints = exam.Tasks.Sum(x => x.MaxPoints);
        var achievedPoints = submission.Answers.Sum(x => x.AchievedPoints!.Value);
        var interval = exam.GradingTable!.Intervals.Single(x => x.Includes(achievedPoints));
        var grade = interval.Grade;

        var pointsText = $"Erzielte Punkte: {achievedPoints}/{maxPoints}";
        var pointsInterval =
            $"Note: \"{grade}\" im Intervall {(interval.Start.Type == GradingTableIntervalBoundType.Exclusive ? "(" : "[")}{interval.Start.Points};{interval.End.Points}{(interval.End.Type == GradingTableIntervalBoundType.Exclusive ? ")" : "]")}";

        var pdf = _pdfService.CreatePdfWithText($"{pointsText}\n{pointsInterval}");

        return pdf;
    }
}