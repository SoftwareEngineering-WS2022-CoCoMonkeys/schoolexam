using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SchoolExam.Core.Domain.CourseAggregate;

namespace SchoolExam.Web.Course
{
    [ApiController]
    [Route("[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly ILogger<CourseController> _logger;
        private readonly IMapper _mapper;
        private readonly ICourseRepository _courseRepository;

        public CourseController(ILogger<CourseController> logger, IMapper mapper, ICourseRepository courseRepository)
        {
            _logger = logger;
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