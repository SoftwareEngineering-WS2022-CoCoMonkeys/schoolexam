using AutoMapper;
using SchoolExam.Application.Services;
using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.Entities.SubmissionAggregate;
using SchoolExam.Web.Models.Course;
using SchoolExam.Web.Models.Exam;

namespace SchoolExam.Web.Mapping;

public class SchoolExamMappingProfile : Profile
{
    public SchoolExamMappingProfile()
    {
        CreateMap<Course, CourseReadModelBase>()
            .Include<Course, CourseReadModelStudent>()
            .Include<Course, CourseReadModelTeacher>()
            .ForMember(dst => dst.Subject, opt => opt.PreCondition(src => src.Subject != null))
            .ForMember(dst => dst.Subject, opt => opt.MapFrom(src => src.Subject!.Name));
        CreateMap<Course, CourseReadModelStudent>();
        CreateMap<Course, CourseReadModelTeacher>()
            .ForMember(dst => dst.StudentCount, opt => opt.MapFrom(src => src.Students.Count));

        CreateMap<SubmissionPage, UnmatchedSubmissionPageReadModel>()
            .ForMember(dst => dst.Size, opt => opt.MapFrom(src => src.PdfFile.Size))
            .ForMember(dst => dst.FileId, opt => opt.MapFrom(src => src.PdfFile.Id))
            .ForMember(dst => dst.UploadedAt, opt => opt.MapFrom(src => src.PdfFile.UploadedAt))
            .ForMember(dst => dst.UploadedBy, opt => opt.MapFrom(src => src.PdfFile.Uploader.Username));
        CreateMap<ExamBookletPage, UnmatchedBookletPageReadModel>();

        CreateMap<Exam, ExamReadModelTeacher>()
            .ForMember(dst => dst.ParticipantCount, opt => opt.MapFrom(src => src.Course.Students.Count))
            .ForMember(dst => dst.CorrectionProgress, opt => opt.MapFrom(src => src.GetCorrectionProgress()))
            .ForMember(dst => dst.Subject, opt => opt.PreCondition(src => src.Course.Subject != null))
            .ForMember(dst => dst.Subject, opt => opt.MapFrom(src => src.Course.Subject!.Name));

        CreateMap<ExamTaskModel, ExamTaskInfo>();

        CreateMap<Submission, SubmissionReadModel>()
            .ForMember(x => x.IsMatched, opt => opt.MapFrom(src => src.Student != null))
            .ForMember(x => x.Student,
                opt => opt.MapFrom(
                    src => src.Student != null ? $"{src.Student.FirstName} {src.Student.LastName}" : null));
    }
}