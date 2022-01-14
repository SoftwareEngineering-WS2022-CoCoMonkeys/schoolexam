using AutoMapper;
using SchoolExam.Application.Services;
using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Web.Models.Course;
using SchoolExam.Web.Models.Exam;
using SchoolExam.Web.Models.Submission;

namespace SchoolExam.Web.Mapping;

public class SchoolExamMappingProfile : Profile
{
    public SchoolExamMappingProfile()
    {
        CreateMap<Course, CourseReadModelBase>()
            .Include<Course, CourseReadModelStudent>()
            .Include<Course, CourseReadModelTeacher>()
            .ForMember(dst => dst.Topic, opt => opt.PreCondition(src => src.Topic != null))
            .ForMember(dst => dst.Topic, opt => opt.MapFrom(src => src.Topic!.Name));
        CreateMap<Course, CourseReadModelStudent>();
        CreateMap<Course, CourseReadModelTeacher>()
            .ForMember(dst => dst.StudentCount, opt => opt.MapFrom(src => src.Students.Count));

        CreateMap<SubmissionPage, UnmatchedSubmissionPageReadModel>()
            .ForMember(dst => dst.Size, opt => opt.MapFrom(src => src.PdfFile.Size))
            .ForMember(dst => dst.FileId, opt => opt.MapFrom(src => src.PdfFile.Id))
            .ForMember(dst => dst.UploadedAt, opt => opt.MapFrom(src => src.PdfFile.UploadedAt))
            .ForMember(dst => dst.UploadedBy, opt => opt.MapFrom(src => src.PdfFile.Uploader.Username));
        CreateMap<BookletPage, UnmatchedBookletPageReadModel>();

        CreateMap<Exam, ExamReadModelTeacher>()
            .ForMember(dst => dst.Status, opt => opt.MapFrom(src => src.State))
            .ForMember(dst => dst.Quota, opt => opt.MapFrom(src => src.GetCorrectionProgress()))
            .ForMember(dst => dst.Topic, opt => opt.MapFrom(src => src.Topic.Name));
        CreateMap<ExamTask, ExamTaskReadModel>();
        CreateMap<ExamParticipant, ExamParticipantReadModel>()
            .Include<ExamCourse, ExamCourseReadModel>()
            .Include<ExamStudent, ExamStudentReadModel>();
        CreateMap<ExamCourse, ExamCourseReadModel>()
            .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.ParticipantId))
            .ForMember(dst => dst.DisplayName, opt => opt.MapFrom(src => src.Course.Name))
            .ForMember(dst => dst.Children, opt => opt.MapFrom(src => src.Course.Students.Select(x => x.Student)));
        CreateMap<Student, ExamStudentReadModel>()
            .ForMember(dst => dst.DisplayName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
        CreateMap<ExamStudent, ExamStudentReadModel>()
            .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.ParticipantId))
            .ForMember(dst => dst.DisplayName,
                opt => opt.MapFrom(src => $"{src.Student.FirstName} {src.Student.LastName}"));

        CreateMap<ExamTaskWriteModel, ExamTaskInfo>();

        CreateMap<Submission, SubmissionReadModel>()
            .Include<Submission, SubmissionDetailsReadModel>()
            .ForMember(dst => dst.AchievedPoints, opt => opt.MapFrom(src => src.Answers.Sum(x => x.AchievedPoints)))
            .ForMember(dst => dst.Status, opt => opt.MapFrom(src => GetCorrectionState(src)))
            .ForMember(dst => dst.IsComplete, opt => opt.MapFrom(src => src.PdfFile != null))
            .ForMember(dst => dst.IsMatchedToStudent, opt => opt.MapFrom(src => src.Student != null));
        CreateMap<Submission, SubmissionDetailsReadModel>()
            .ForMember(dst => dst.Data,
                opt => opt.MapFrom(src => src.PdfFile != null ? Convert.ToBase64String(src.PdfFile.Content) : null));
        CreateMap<Answer, AnswerReadModel>()
            .ForMember(dst => dst.Status, opt => opt.MapFrom(src => GetCorrectionState(src)));
        CreateMap<AnswerSegment, AnswerSegmentReadModel>();
        CreateMap<ExamPosition, SegmentPositionReadModel>();
    }

    private CorrectionState GetCorrectionState(Submission submission)
    {
        var countCorrected = submission.Answers.Count(x => x.State == AnswerState.Corrected);
        var countPending = submission.Answers.Count(x => x.State == AnswerState.Pending);
        var count = submission.Answers.Count;

        if (count == countPending)
        {
            return CorrectionState.Pending;
        }

        if (count == countCorrected)
        {
            return CorrectionState.Corrected;
        }

        return CorrectionState.InProgress;
    }

    private CorrectionState GetCorrectionState(Answer answer)
    {
        return answer.State == AnswerState.Corrected ? CorrectionState.Corrected : CorrectionState.Pending;
    }
}