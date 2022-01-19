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
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.IntegrationTests.Util;

public class AutoFixtureTestEntityFactory : ISchoolExamTestEntityFactory,
    ITestEntityFactory<School>,
    ITestEntityFactory<Course>,
    ITestEntityFactory<Teacher>,
    ITestEntityFactory<Student>,
    ITestEntityFactory<LegalGuardian>,
    ITestEntityFactory<Exam>,
    ITestEntityFactory<ExamTask>,
    ITestEntityFactory<User>,
    ITestEntityFactory<TaskPdfFile>,
    ITestEntityFactory<Submission>,
    ITestEntityFactory<SubmissionPagePdfFile>,
    ITestEntityFactory<SubmissionPage>,
    ITestEntityFactory<Answer>,
    ITestEntityFactory<AnswerSegment>,
    ITestEntityFactory<Booklet>,
    ITestEntityFactory<BookletPage>,
    ITestEntityFactory<BookletPdfFile>
{
    private readonly Fixture _fixture;

    public AutoFixtureTestEntityFactory()
    {
        _fixture = new Fixture();

        _fixture.Customize<School>(opts => opts.Without(x => x.Teachers));
        _fixture.Customize<Course>(opts => opts.Without(x => x.Students).Without(x => x.Teachers));
        _fixture.Customize<Student>(opts =>
            opts.Without(x => x.Courses).Without(x => x.LegalGuardians).Without(x => x.User));
        _fixture.Customize<Teacher>(opts => opts.Without(x => x.Courses).Without(x => x.User));
        _fixture.Customize<LegalGuardian>(opts => opts.Without(x => x.Children).Without(x => x.User));
        _fixture.Customize<Exam>(opts =>
            opts.With(x => x.State, ExamState.Planned).Without(x => x.Booklets).Without(x => x.Tasks)
                .Without(x => x.Participants));
        _fixture.Customize<GradingTableInterval>(opts => opts.Without(x => x.GradingTable));
        _fixture.Customize<TaskPdfFile>(opts => opts.With(x => x.Content, CreatePdfFile()));
        _fixture.Customize<Booklet>(opts => opts.Without(x => x.Pages).Without(x => x.Exam));
        _fixture.Customize<BookletPdfFile>(opts => opts.With(x => x.Content, CreatePdfFile()));
        _fixture.Customize<Submission>(opts =>
            opts.Without(x => x.Pages).Without(x => x.Booklet).Without(x => x.Answers).Without(x => x.Student));
        _fixture.Customize<SubmissionPagePdfFile>(opts => opts.With(x => x.Content, CreatePdfFile()));
        _fixture.Customize<BookletPage>(opts => opts.Without(x => x.SubmissionPage));
        _fixture.Customize<Answer>(opts => opts.Without(x => x.Task));
        _fixture.Customize<RemarkPdfFile>(opts => opts.With(x => x.Content, CreatePdfFile()));
        _fixture.Customize<User>(opts => opts.Without(x => x.Person));
    }

    public TEntity Create<TEntity>() where TEntity : IEntity
    {
        ITestEntityFactory<TEntity>? factory = this as ITestEntityFactory<TEntity>;
        if (factory == null)
        {
            throw new ArgumentException($"No factory method implemented for entity type {typeof(TEntity).Name}.");
        }

        return factory.Create();
    }

    School ITestEntityFactory<School>.Create()
    {
        return _fixture.Create<School>();
    }

    Course ITestEntityFactory<Course>.Create()
    {
        return _fixture.Create<Course>();
    }

    Teacher ITestEntityFactory<Teacher>.Create()
    {
        return _fixture.Create<Teacher>();
    }

    Student ITestEntityFactory<Student>.Create()
    {
        return _fixture.Create<Student>();
    }
    
    LegalGuardian ITestEntityFactory<LegalGuardian>.Create()
    {
        return _fixture.Create<LegalGuardian>();
    }

    public Exam Create()
    {
        return _fixture.Create<Exam>();
    }

    ExamTask ITestEntityFactory<ExamTask>.Create()
    {
        return _fixture.Create<ExamTask>();
    }

    User ITestEntityFactory<User>.Create()
    {
        return _fixture.Create<User>();
    }

    TaskPdfFile ITestEntityFactory<TaskPdfFile>.Create()
    {
        return _fixture.Create<TaskPdfFile>();
    }

    SubmissionPagePdfFile ITestEntityFactory<SubmissionPagePdfFile>.Create()
    {
        return _fixture.Create<SubmissionPagePdfFile>();
    }

    SubmissionPage ITestEntityFactory<SubmissionPage>.Create()
    {
        return _fixture.Create<SubmissionPage>();
    }

    Booklet ITestEntityFactory<Booklet>.Create()
    {
        return _fixture.Create<Booklet>();
    }

    BookletPage ITestEntityFactory<BookletPage>.Create()
    {
        return _fixture.Create<BookletPage>();
    }

    Submission ITestEntityFactory<Submission>.Create()
    {
        return _fixture.Create<Submission>();
    }

    BookletPdfFile ITestEntityFactory<BookletPdfFile>.Create()
    {
        return _fixture.Create<BookletPdfFile>();
    }

    Answer ITestEntityFactory<Answer>.Create()
    {
        return _fixture.Create<Answer>();
    }

    AnswerSegment ITestEntityFactory<AnswerSegment>.Create()
    {
        return _fixture.Create<AnswerSegment>();
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