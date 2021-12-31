using System;
using System.IO;
using System.Linq;
using AutoFixture;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using SchoolExam.Domain.Base;
using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.Entities.SchoolAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;
using SchoolExam.Domain.Entities.UserAggregate;

namespace SchoolExam.IntegrationTests.Util;

public class AutoFixtureTestEntityFactory : ISchoolExamTestEntityFactory,
    ITestEntityFactory<School, Guid>,
    ITestEntityFactory<Course, Guid>,
    ITestEntityFactory<Teacher, Guid>,
    ITestEntityFactory<Student, Guid>,
    ITestEntityFactory<Exam, Guid>,
    ITestEntityFactory<User, Guid>,
    ITestEntityFactory<TaskPdfFile, Guid>,
    ITestEntityFactory<Submission, Guid>,
    ITestEntityFactory<SubmissionPagePdfFile, Guid>,
    ITestEntityFactory<SubmissionPage, Guid>,
    ITestEntityFactory<ExamBooklet, Guid>,
    ITestEntityFactory<ExamBookletPage, Guid>
{
    private readonly Fixture _fixture;

    public AutoFixtureTestEntityFactory()
    {
        _fixture = new Fixture();

        _fixture.Customize<ExamBooklet>(opts => opts.Without(x => x.Pages));
        _fixture.Customize<Submission>(opts => opts.Without(x => x.Pages));
        _fixture.Customize<ExamBookletPage>(opts => opts.Without(x => x.SubmissionPage));
    }

    public TEntity Create<TEntity, TIdentity>() where TEntity : IEntity<TIdentity>
    {
        ITestEntityFactory<TEntity, TIdentity>? factory = this as ITestEntityFactory<TEntity, TIdentity>;
        if (factory == null)
        {
            throw new ArgumentException($"No factory method implemented for entity type {typeof(TEntity).Name}.");
        }

        return factory.Create();
    }

    School ITestEntityFactory<School, Guid>.Create()
    {
        return _fixture.Create<School>();
    }

    Course ITestEntityFactory<Course, Guid>.Create()
    {
        return _fixture.Create<Course>();
    }

    Teacher ITestEntityFactory<Teacher, Guid>.Create()
    {
        return _fixture.Create<Teacher>();
    }

    Student ITestEntityFactory<Student, Guid>.Create()
    {
        return _fixture.Create<Student>();
    }

    public Exam Create()
    {
        return _fixture.Build<Exam>().Without(x => x.Booklets).Without(x => x.Tasks).Without(x => x.GradingTable)
            .Without(x => x.TaskPdfFile).Create();
    }

    User ITestEntityFactory<User, Guid>.Create()
    {
        return _fixture.Create<User>();
    }

    TaskPdfFile ITestEntityFactory<TaskPdfFile, Guid>.Create()
    {
        return _fixture.Build<TaskPdfFile>().With(x => x.Content, CreatePdfFile()).Create();
    }
    
    SubmissionPagePdfFile ITestEntityFactory<SubmissionPagePdfFile, Guid>.Create()
    {
        return _fixture.Build<SubmissionPagePdfFile>().With(x => x.Content, CreatePdfFile()).Create();
    }
    
    SubmissionPage ITestEntityFactory<SubmissionPage, Guid>.Create()
    {
        return _fixture.Create<SubmissionPage>();
    }
    
    ExamBooklet ITestEntityFactory<ExamBooklet, Guid>.Create()
    {
        return _fixture.Create<ExamBooklet>();
    }

    ExamBookletPage ITestEntityFactory<ExamBookletPage, Guid>.Create()
    {
        return _fixture.Create<ExamBookletPage>();
    }
    
    Submission ITestEntityFactory<Submission, Guid>.Create()
    {
        return _fixture.Create<Submission>();
    }

    private byte[] CreatePdfFile()
    {
        var stream = new MemoryStream();
        var writer = new PdfWriter(stream);
        var pdf = new PdfDocument(writer);
        var document = new Document(pdf);

        document.Add(new Paragraph(_fixture.Create<string>()));
        document.Add(new AreaBreak());
        document.Add(new Paragraph(_fixture.Create<string>()));
        document.Close();

        var result = stream.ToArray();

        return result.ToArray();
    }
}