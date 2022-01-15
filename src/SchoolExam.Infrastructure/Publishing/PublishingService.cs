using System.Collections;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using SchoolExam.Application.Publishing;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.PersonAggregate;

namespace SchoolExam.Infrastructure.Publishing;

public class EmailCreator : IEmailCreator
{
    public bool sendEmailToStudent(Student student, Exam exam)
    {

        // Specify the file to be attached and sent.
        // This example assumes that a file named Data.xls exists in the
        // current working directory.
        string file = "data.xls";
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


        // Create  the file attachment for this email message.
        Attachment data = new Attachment(file, MediaTypeNames.Application.Octet);
        // Add time stamp information for the file.
        ContentDisposition disposition = data.ContentDisposition;
        disposition.CreationDate = System.IO.File.GetCreationTime(file);
        disposition.ModificationDate = System.IO.File.GetLastWriteTime(file);
        disposition.ReadDate = System.IO.File.GetLastAccessTime(file);
        // Add the file attachment to this email message.
        message.Attachments.Add(data);

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

        data.Dispose();
        return true;
    }

    public async Task scheduleSendEmailToStudent(IEnumerable<Student>  student, Exam exam, DateTime publishDateTime)
    {
           
    }
}