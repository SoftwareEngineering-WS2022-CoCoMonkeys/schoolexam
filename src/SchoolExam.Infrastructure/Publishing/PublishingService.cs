﻿using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using Microsoft.Extensions.Logging;
using MimeKit;
using SchoolExam.Application.Pdf;
using SchoolExam.Application.Publishing;
using SchoolExam.Application.Repository;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.Infrastructure.Publishing;

public class PublishingService : IPublishingService
{
    private readonly ISchoolExamRepository _repository;
    private readonly IPdfService _pdfService;
    private Timer _timer;
    private readonly ILogger<PublishingService> _logger;

    public PublishingService(ISchoolExamRepository repository, IPdfService pdfService, ILogger<PublishingService> logger)
    {
        _repository = repository;
        _pdfService = pdfService;
        _logger = logger;
    }
    
    public bool SendEmailToStudent(Booklet booklet, Exam exam)
    {
        var student = booklet.Submission!.Student;
        var remarkPdf = booklet.Submission.RemarkPdfFile!;
        
        // Create a message and set up the recipients.
        var mailSubject =
            string.Format(
                $"Deine Note in {exam.Title} am {exam.Date.Day}.{exam.Date.Month}.{exam.Date.Year} {exam.Topic.Name} ");
        var mailLine1 = string.Format($"Hallo {student.FirstName} {student.LastName} !");
        var mailLine2 =
            string.Format(
                $"Dein Prüfungsergebnis in {exam.Title} am {exam.Date.Day}.{exam.Date.Month}.{exam.Date.Year} wurde gerade veröffentlicht!");
        var mailLine3 =
            string.Format("Du findest deine Note sowie die korrigierte Prüfung in der PDF-Datei im Anhang!");
        var mailLine4 =
            string.Format(
                "Diese Mail wurde automatisch von SchoolExam erstellt. Bitte wende dich bei Fragen an deine Lehrkraft.");

        MailMessage message = new MailMessage(
            "schoolexam@rootitup.de",
            student.EmailAddress,
            mailSubject,
            string.Format($"{mailLine1}\n\n{mailLine2}\n{mailLine3}\n\n{mailLine4}"));

        var messageMailKit = new MimeMessage();
        messageMailKit.From.Add(new MailboxAddress("SchoolExam", "schoolexam@rootitup.de"));

        messageMailKit.To.Add(new MailboxAddress($"{student.FirstName} {student.LastName}", student.EmailAddress));

        messageMailKit.Subject = mailSubject;
        messageMailKit.Body = new TextPart("html") { Text = string.Format($"{mailLine1}\n\n{mailLine2}\n{mailLine3}\n\n{mailLine4}")};

        var examToBePublishedPdf = GetExamPdfToBePublished(remarkPdf, student);
            
        using var stream = new MemoryStream(examToBePublishedPdf);
        // Create  the file attachment for this email message.
        var attachment = new Attachment(stream,
            $"Prüfungskorrektur_{student.FirstName}_{exam.Title}_{exam.Date.Day}.{exam.Date.Month}.{exam.Date.Year}.pdf");//MediaTypeNames.Application.Pdf);
        message.Attachments.Add(attachment);

        using MailKit.Net.Smtp.SmtpClient mailKitClient = new MailKit.Net.Smtp.SmtpClient();
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

        attachment.Dispose();
        return true;
    }
    
    private byte[] GetExamPdfToBePublished(RemarkPdfFile remarkPdf, Student student)
    {
        var userPassword  = $"{student.Id.ToString().Substring(0,12)}";
        var ownerPassword = $"{student.Id}";
        var protectedPdf = _pdfService.Protect(remarkPdf.Content, Encoding.UTF8.GetBytes(userPassword),
            Encoding.UTF8.GetBytes(ownerPassword));
        return protectedPdf;
    }

    public async Task ScheduleSendEmailToStudent(IEnumerable<Booklet>  booklets, Exam exam, DateTime publishDateTime)
    {
        DateTime current = DateTime.Now;
        TimeSpan timeToGo = publishDateTime - current;
        if (timeToGo < TimeSpan.Zero)
        {
            await DoPublishExam(booklets, exam);
            return;
        }
        _timer = new Timer(async x =>
        {
            await DoPublishExam(booklets, exam);
        }, null, timeToGo, Timeout.InfiniteTimeSpan);
    }

    public async Task DoPublishExam(IEnumerable<Booklet> booklets, Exam exam)
    {
        foreach(Booklet booklet in booklets)
        {
           SendEmailToStudent(booklet, exam);
                
        }
        exam.State = ExamState.Published;
        _repository.Update(exam);
        await _repository.SaveChangesAsync();
    }
    
}