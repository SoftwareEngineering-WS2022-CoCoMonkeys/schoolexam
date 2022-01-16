using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.EntityFrameworkCore;
using SchoolExam.Application.Pdf;
using SchoolExam.Application.QrCode;
using SchoolExam.Application.RandomGenerator;
using SchoolExam.Application.Repository;
using SchoolExam.Application.Services;
using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.Entities.SchoolAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;
using SchoolExam.Domain.Entities.UserAggregate;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Extensions;
using SchoolExam.Persistence.DataContext;

namespace SchoolExam.Infrastructure.Repository;

public class SchoolExamRepositoryInitService : ISchoolExamRepositoryInitService
{
    private Guid _gymnasiumDiedorfId = Guid.Parse("ae6c71b1-9bb8-4272-812d-7268ac419242"),
        _brigitteSchweinebauerId = Guid.Parse("0e40059f-967a-404e-915c-c4c862e471ef"),
        _brigitteSchweinebauerUserId = Guid.Parse("baa9a10f-eb07-4bf3-8438-48adaa44f6c0"),
        _adminUserId = Guid.Parse("26bb9bc0-0610-4fb9-bdb5-05fef1243863"),
        _sozialwissenschaftenCourseId = Guid.Parse("481b9bd0-923f-44bd-8741-867a60471ee5"),
        _naturwissenschaftenCourseId = Guid.Parse("d2f5cf0d-5974-4574-b3fb-b04044d8db76");

    private readonly SchoolExamDbContext _context;
    private readonly Random _random = new(16012022);
    private readonly IRandomGenerator _randomGenerator;
    private readonly IExamService _examService;
    private readonly ISubmissionService _submissionService;
    private readonly IPdfService _pdfService;
    private readonly IQrCodeGenerator _qrCodeGenerator;

    private static string[] _firstNames => new[]
    {
        "Matteo", "Emilia", "Noah", "Hannah", "Leon", "Mia", "Finn", "Emma", "Elias", "Sophia", "Paul", "Mila", "Ben",
        "Lina", "Luca", "Ella", "Emil", "Lea", "Louis", "Marie"
    };

    private static string[] _lastNames => new[]
    {
        "Müller", "Schmidt", "Schneider", "Fischer", "Weber", "Meyer", "Wagner", "Becker", "Schulz", "Hoffmann",
        "Schäfer", "Koch", "Bauer", "Richter", "Klein", "Wolf", "Schröder", "Neumann", "Schwarz", "Zimmermann"
    };

    private static string[] _examTitles => new[]
    {
        "Mitarbeitskontrolle", "Endterm", "Midterm", "Prüfung", "Klausur", "Schulaufgabe", "Test", "Retake"
    };

    private static string[] _topicsSozialwissenschaften => new[]
    {
        "Geschichte", "Politik", "Sozialkunde"
    };

    private static string[] _topicsNaturwissenschaften => new[]
    {
        "Mathematik", "Physik", "Chemie", "Informatik"
    };

    private static int _minYear = 2004;
    private static int _maxYear = 2008;

    public SchoolExamRepositoryInitService(SchoolExamDbContext context, IExamService examService,
        ISubmissionService submissionService, IPdfService pdfService, IQrCodeGenerator qrCodeGenerator)
    {
        _context = context;
        _randomGenerator = new RandomGenerator.RandomGenerator(_random);
        _examService = examService;
        _submissionService = submissionService;
        _pdfService = pdfService;
        _qrCodeGenerator = qrCodeGenerator;
    }

    public async Task Init()
    {
        await _context.Database.MigrateAsync();
        // clear database tables
        _context.RemoveRange(_context.Set<SubmissionPage>());
        _context.RemoveRange(_context.Set<Submission>());
        _context.RemoveRange(_context.Set<Booklet>());
        _context.RemoveRange(_context.Set<Exam>());
        _context.RemoveRange(_context.Set<User>());
        _context.RemoveRange(_context.Set<Student>());
        _context.RemoveRange(_context.Set<Teacher>());
        _context.RemoveRange(_context.Set<LegalGuardian>());
        _context.RemoveRange(_context.Set<Course>());
        _context.RemoveRange(_context.Set<School>());
        await _context.SaveChangesAsync();
        
        // fill database tables
        await AddSchools();
        await AddCourses();
        await AddTeachers();
        await AddUsers();
        var studentIds = await AddStudents();
        await AddExams(studentIds);
        await _context.SaveChangesAsync();
    }

    private async Task AddSchools()
    {
        var school = new School(_gymnasiumDiedorfId, "Schmuttertal-Gymnasium Diedorf",
            new Address("Schmetterlingsplatz", "1", "86420", "Diedorf", "Deutschland"));
        _context.Add(school);
    }

    private async Task AddCourses()
    {
        var courseSozialwissenschaften = new Course(_sozialwissenschaftenCourseId, "Sozialwissenschaften",
            new Topic("Projektmanagement"), _gymnasiumDiedorfId);
        _context.Add(courseSozialwissenschaften);
        var courseNaturwissenschaften = new Course(_naturwissenschaftenCourseId, "Naturwissenschaften",
            new Topic("Physik"), _gymnasiumDiedorfId);
        _context.Add(courseNaturwissenschaften);
    }

    private async Task AddTeachers()
    {
        var teacher = new Teacher(_brigitteSchweinebauerId, "Brigitte", "Schweinebauer",
            new DateTime(1974, 5, 18).SetKindUtc(),
            new Address("Klarer-Kopf-Weg", "1a", "20095", "Hamburg", "Deutschland"),
            "brigitte.schweinebauer@schoolexam.de", _gymnasiumDiedorfId);
        _context.Add(teacher);
        var courseTeacherSozialwissenschaften =
            new CourseTeacher(_sozialwissenschaftenCourseId, _brigitteSchweinebauerId);
        _context.Add(courseTeacherSozialwissenschaften);
        var courseTeacherNaturwissenschaften =
            new CourseTeacher(_naturwissenschaftenCourseId, _brigitteSchweinebauerId);
        _context.Add(courseTeacherNaturwissenschaften);

        await _context.SaveChangesAsync();
    }

    private async Task AddUsers()
    {
        var userBrigitte = new User(_brigitteSchweinebauerUserId, "brigitte",
            "$2a$11$3Q8Re.PhjBIPqPIqzAy3Y./XFRjcelEOr7kL0X27ljVbay1PwTMw2", Role.Teacher, _brigitteSchweinebauerId);
        var userAdmin = new User(_adminUserId, "admin", "2a$11$3Q8Re.PhjBIPqPIqzAy3Y./XFRjcelEOr7kL0X27ljVbay1PwTMw2",
            Role.Administrator, null);
        _context.Add(userBrigitte);
        _context.Add(userAdmin);

        await _context.SaveChangesAsync();
    }

    private async Task<IEnumerable<Guid>> AddStudents()
    {
        var count = 100;
        var countFirstNames = _firstNames.Length;
        var countLastNames = _lastNames.Length;

        Student GenerateStudent()
        {
            var firstName = _firstNames[_random.Next(countFirstNames)];
            var lastName = _lastNames[_random.Next(countLastNames)];
            var dateOfBirth = CreateRandomDate(_minYear, _maxYear);
            var emailAddress = $"{firstName}.{lastName}@schoolexam.de";
            var qrCode = _randomGenerator.GenerateHexString(32);
            return new Student(Guid.NewGuid(), firstName, lastName, dateOfBirth, new Address("", "", "", "", ""),
                emailAddress, qrCode, _gymnasiumDiedorfId);
        }

        var students = Enumerable.Range(0, count).Select(_ => GenerateStudent()).ToList();
        _context.AddRange(students);
        var courseSozialwissenschaftenStudents = students.OrderBy(_ => _random.Next()).Take(20)
            .Select(x => new CourseStudent(_sozialwissenschaftenCourseId, x.Id));
        _context.AddRange(courseSozialwissenschaftenStudents);
        var courseNaturwissenschaftenStudents = students.OrderBy(_ => _random.Next()).Take(20)
            .Select(x => new CourseStudent(_naturwissenschaftenCourseId, x.Id));
        _context.AddRange(courseNaturwissenschaftenStudents);

        await _context.SaveChangesAsync();

        return students.Select(x => x.Id);
    }

    private async Task AddExams(IEnumerable<Guid> studentIds)
    {
        var count = 10;
        var countTitles = _examTitles.Length;

        var examIds = Enumerable.Range(0, count).Select(_ => Guid.NewGuid()).ToArray();
        for (int i = 0; i < count; i++)
        {
            var isSozialwissenschaften = _random.Next(2) == 0;
            var topics = isSozialwissenschaften ? _topicsSozialwissenschaften : _topicsNaturwissenschaften;
            var countTopics = topics.Length;
            var examId = examIds[i];
            var topic = topics[_random.Next(countTopics)];
            var title = $"{_examTitles[_random.Next(countTitles)]} {topic} {_random.Next(1, 10)}";
            var date = DateTime.Now.AddDays(_random.Next(7, 56)).SetKindUtc();
            var exam = new Exam(examId, title, date, _brigitteSchweinebauerId, new Topic(topic));
            _context.Add(exam);

            // additional students
            var studentCount = _random.Next(5);
            var additionalStudents = studentIds.OrderBy(_ => _random.Next()).Take(studentCount);
            var examStudents = additionalStudents.Select(x => new ExamStudent(examId, x));
            _context.AddRange(examStudents);

            // add one course to exam
            var courseId = isSozialwissenschaften ? _sozialwissenschaftenCourseId : _naturwissenschaftenCourseId;
            var examCourse = new ExamCourse(examId, courseId);
            _context.Add(examCourse);
        }

        await _context.SaveChangesAsync();

        for (var i = 0; i < count; i++)
        {
            var examId = examIds[i];
            var states = Enum.GetValues(typeof(ExamState));
            var state = (ExamState) (states.GetValue(_random.Next(states.Length - 1)) ?? 0);
            if (state == ExamState.Planned)
            {
                continue;
            }

            // create tasks and task PDF file
            var taskCount = _random.Next(10);
            var taskIds = Enumerable.Range(0, taskCount).Select(_ => Guid.NewGuid()).ToArray();
            var examTasks = new List<ExamTaskInfo>();
            for (int j = 1; j <= taskCount; j++)
            {
                var taskTitle = $"Aufgabe {j}";
                var maxPoints = _random.Next(10);
                var examTask = new ExamTaskInfo(taskIds[j - 1], taskTitle, maxPoints);
                examTasks.Add(examTask);
            }

            var taskPdfFile = CreateTaskPdfFile(taskIds.ToArray());
            await _examService.SetTaskPdfFile(examId, _brigitteSchweinebauerUserId, taskPdfFile);
            await _examService.FindTasks(examId, _brigitteSchweinebauerUserId, examTasks.ToArray());

            if (state == ExamState.BuildReady)
            {
                continue;
            }

            await _examService.Build(examId, _brigitteSchweinebauerUserId);

            if (state == ExamState.SubmissionReady)
            {
                continue;
            }

            var booklets = _context.Set<Booklet>().Include(x => x.PdfFile).Where(x => x.ExamId.Equals(examId))
                .ToArray();
            var examStudents = _context.Set<ExamStudent>().Include(x => x.Student).Where(x => x.ExamId.Equals(examId))
                .AsEnumerable();
            var examCourses = _context.Set<ExamCourse>().Include(x => x.Course).ThenInclude(x => x.Students)
                .ThenInclude(x => x.Student).Where(x => x.ExamId.Equals(examId)).AsEnumerable();
            var students = examStudents.Select(x => x.Student)
                .Union(examCourses.SelectMany(x => x.Course.Students).Select(x => x.Student))
                .DistinctBy(x => x.Id)
                .OrderBy(_ => _random.Next())
                .ToArray();

            var submissionCount = booklets.Length;
            var submissionPdfs = new byte[submissionCount][];
            for (int j = 0; j < submissionCount; j++)
            {
                var booklet = booklets[j];
                var student = students[j];
                var bookletPdf = booklet.PdfFile.Content;
                var qrCode = _qrCodeGenerator.GeneratePngQrCode(student.QrCode.Data);
                var qrCodeRenderInfo = new PdfImageRenderInfo(1, 100, 500, 100, qrCode);
                var pdfWithQrCode = _pdfService.RenderImages(bookletPdf, qrCodeRenderInfo);
                submissionPdfs[j] = pdfWithQrCode;
            }

            var submissionPdf = _pdfService.Merge(submissionPdfs);
            await _examService.Match(examId, submissionPdf, _brigitteSchweinebauerUserId);

            var submissions = _submissionService.GetByExam(examId);
            foreach (var submission in submissions)
            {
                foreach (var examTask in examTasks)
                {
                    var isCorrected = _random.Next(2) == 0;
                    if (isCorrected || state is ExamState.Corrected)
                    {
                        await _submissionService.SetPoints(submission.Id, examTask.Id,
                            Math.Round(_random.NextDouble() * examTask.MaxPoints));
                    }
                }
            }

            if (state is ExamState.InCorrection or ExamState.Corrected)
            {
                continue;
            }
            
             // TODO published
        }
    }

    private DateTime CreateRandomDate(int minYear, int maxYear)
    {
        DateTimeOffset minDate = new DateTime(minYear, 1, 1);
        DateTimeOffset maxDate = new DateTime(maxYear, 12, 31);
        long min = minDate.UtcTicks;
        long max = maxDate.UtcTicks;
        long date = min + _random.NextInt64(max - min);
        return new DateTimeOffset(date, TimeSpan.Zero).DateTime.SetKindUtc();
    }

    private byte[] CreateTaskPdfFile(Guid[] taskIds)
    {
        var stream = new MemoryStream();
        var writer = new PdfWriter(stream);
        var pdf = new PdfDocument(writer);
        var document = new Document(pdf);

        for (int i = 0; i < taskIds.Length; i++)
        {
            var taskId = taskIds[i];
            var actionStart = PdfAction.CreateURI($"task-start-{taskId}");
            var linkStart = new Link("Start", actionStart);
            var text = _randomGenerator.GenerateHexString(200);
            document.Add(new Paragraph().Add(linkStart).Add(text));
            var actionEnd = PdfAction.CreateURI($"task-end-{taskId}");
            var linkEnd = new Link("End", actionEnd);
            document.Add(new Paragraph().Add(linkEnd));
            var hasPageBreak = _random.Next(2) == 0;
            if (i < taskIds.Length - 1 && hasPageBreak)
            {
                document.Add(new AreaBreak());
            }
        }

        document.Close();

        var result = stream.ToArray();

        return result;
    }
}