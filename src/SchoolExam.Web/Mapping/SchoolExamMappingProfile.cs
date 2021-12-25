using AutoMapper;
using SchoolExam.Web.Course;

namespace SchoolExam.Web.Mapping
{
    public class SchoolExamMappingProfile : Profile
    {
        public SchoolExamMappingProfile()
        {
            CreateMap<Domain.Entities.CourseAggregate.Course, CourseReadModelBase>()
                .Include<Domain.Entities.CourseAggregate.Course, CourseReadModelStudent>()
                .Include<Domain.Entities.CourseAggregate.Course, CourseReadModelTeacher>()
                .ForMember(dst => dst.Subject, opt => opt.PreCondition(src => src.Subject != null))
                .ForMember(dst => dst.Subject, opt => opt.MapFrom(src => src.Subject!.Name));
            CreateMap<Domain.Entities.CourseAggregate.Course, CourseReadModelStudent>();
            CreateMap<Domain.Entities.CourseAggregate.Course, CourseReadModelTeacher>()
                .ForMember(dst => dst.StudentCount, opt => opt.MapFrom(src => src.StudentIds.Count()));
        }
    }
}