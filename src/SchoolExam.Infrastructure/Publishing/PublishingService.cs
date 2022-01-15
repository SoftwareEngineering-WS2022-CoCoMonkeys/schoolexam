using System.Collections;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.Security;
using SchoolExam.Application.Publishing;
using SchoolExam.Application.Repository;
using SchoolExam.Application.Services;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;
using SchoolExam.Domain.ValueObjects;
using PdfReader = iText.Kernel.Pdf.PdfReader;

namespace SchoolExam.Infrastructure.Publishing;

public class PublishingService : IPublishingService
{
    private readonly ISchoolExamRepository _repository;
    private Timer timer;
    
    public PublishingService(ISchoolExamRepository repository)
    {
        _repository = repository;
    }
    
    public bool sendEmailToStudent(Booklet booklet, Exam exam)
    {

        var student = booklet.Submission.Student;
        var remarkPdf = booklet.Submission.RemarkPdfFile;
        
        // Create a message and set up the recipients.
        var mailSubject =
            string.Format(
                $"Deine Note in {exam.Title} am {exam.Date.Day}.{exam.Date.Month}.{exam.Date.Year} {exam.Topic} ");
        var mailLine1 = string.Format($"Hallo {student.FirstName}!");
        var mailLine2 =
            string.Format(
                $"Deine Prüfung in {exam.Title} am {exam.Date.Day}.{exam.Date.Month}.{exam.Date.Year} wurde gerade veröffentlicht!");
        var mailLine3 =
            string.Format("Du findest deine Note sowie die korrigierte Prüfung in der PDF-Datei im Anhang!");
        var mailLine4 =
            string.Format(
                "Diese Mail wurde automatisch von SchoolExam erstellt. Bitte wende dich bei Fragen an deine Lehrkraft.");

        MailMessage message = new MailMessage(
            "schoolexam@mail01.rootitup.de",
            student.EmailAddress,
            mailSubject,
            string.Format($"{mailLine1}\n\n{mailLine2}\n{mailLine3}\n\n{mailLine4}"));


        var examToBePublishedPdf = GetExamPdfToBePublished(remarkPdf, student);
            
        var attachment = GetPublishingExamAttachment(examToBePublishedPdf);
        message.Attachments.Add(attachment);

        //Send the message.
        SmtpClient client = new SmtpClient
        {
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            EnableSsl = true,
            Host = "mail01.rootitup.de",
            Port = 587,
            Credentials = new NetworkCredential("schoolexam", "DiesesPasswortIstSehrSicher12345#")
        };

        try
        {
            client.Send(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception caught in CreateMessageWithAttachment(): {0}",
                ex.ToString());
        }

        attachment.Dispose();
        return true;
    }

    private Attachment GetPublishingExamAttachment(PdfSharp.Pdf.PdfDocument examToBePublishedPdf)
    {
        
        // Specify the file to be attached and sent.
        // This example assumes that a file named Data.xls exists in the
        // current working directory.
        MemoryStream stream = new MemoryStream();

        examToBePublishedPdf.Save(stream,false);
        
        // Create  the file attachment for this email message.
        Attachment attachment = new Attachment(stream, MediaTypeNames.Application.Octet);
        // Add time stamp information for the file.
        ContentDisposition disposition = attachment.ContentDisposition;
        //disposition.CreationDate = System.IO.File.GetCreationTime(file);
        //disposition.ModificationDate = System.IO.File.GetLastWriteTime(file);
        //disposition.ReadDate = System.IO.File.GetLastAccessTime(file);
        // Add the file attachment to this email message.
        stream.Close();
        return attachment;
    }
    
    private PdfSharp.Pdf.PdfDocument GetExamPdfToBePublished(RemarkPdfFile remarkPdf, Student student)
    {

        //Steam pdf byte array and put in new document to add password
        MemoryStream stream = new MemoryStream(remarkPdf.Content);
        PdfSharp.Pdf.PdfDocument document = PdfSharp.Pdf.IO.PdfReader.Open(stream, PdfDocumentOpenMode.Modify);

        PdfSecuritySettings securitySettings = document.SecuritySettings;

        // Setting one of the passwords automatically sets the security level to 
        // PdfDocumentSecurityLevel.Encrypted128Bit.
        securitySettings.UserPassword  = String.Format($"{student.Id.ToString().Substring(0,12)}");
        securitySettings.OwnerPassword = String.Format($"{student.Id.ToString()}");



        // Restrict some rights.
        /*securitySettings.PermitAccessibilityExtractContent = false;
        securitySettings.PermitAnnotations = false;
        securitySettings.PermitAssembleDocument = false;
        securitySettings.PermitExtractContent = false;
        securitySettings.PermitFormsFill = true;
        securitySettings.PermitFullQualityPrint = false;
        securitySettings.PermitModifyDocument = true;
        securitySettings.PermitPrint = false;*/

        return document;
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
        timer = new Timer(x =>
        {
            DoPublishExam(booklets, exam);
        }, null, timeToGo, Timeout.InfiniteTimeSpan);
    }

    public async Task DoPublishExam(IEnumerable<Booklet> booklets, Exam exam)
    {
        foreach(Booklet booklet in booklets)
        {
           sendEmailToStudent(booklet, exam);
                
        }
        exam.State = ExamState.Published;
        _repository.Update(exam);
        await _repository.SaveChangesAsync();
    }
    
}