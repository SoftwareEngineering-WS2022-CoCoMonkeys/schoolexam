using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SchoolExam.Core.Domain.CourseAggregate;

namespace SchoolExam.Web.Course
{
    public class CourseController : ApiController<CourseController>
    {
        private readonly IMapper _mapper;
        private readonly ICourseRepository _courseRepository;

        public CourseController(ILogger<CourseController> logger, IMapper mapper, ICourseRepository courseRepository) : base(logger)
        {
            _mapper = mapper;
            _courseRepository = courseRepository;
        }

        [HttpGet(Name = "GetAll")]
        public IEnumerable<CourseDto> Get()
        {
            return _mapper.Map<IEnumerable<CourseDto>>(_courseRepository.GetAll());
        }
    }
}