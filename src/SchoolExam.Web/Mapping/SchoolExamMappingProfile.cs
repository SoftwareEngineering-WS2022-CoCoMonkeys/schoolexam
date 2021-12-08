using System.Linq;
using AutoMapper;
using SchoolExam.Web.Course;

namespace SchoolExam.Web.Mapping
{
    public class SchoolExamMappingProfile : Profile
    {
        public SchoolExamMappingProfile()
        {
            CreateMap<Core.Domain.CourseAggregate.Course, CourseDto>()
                .ForMember(dst => dst.Subject, opt => opt.PreCondition(src => src.Subject != null))
                .ForMember(dst => dst.Subject, opt => opt.MapFrom(src => src.Subject!.Name))
                .ForMember(dst => dst.StudentCount, opt => opt.MapFrom(src => src.StudentIds.Count()));
        }
    }
}