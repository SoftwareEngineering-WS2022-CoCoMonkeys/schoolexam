namespace SchoolExam.Web.Course
{
    public class CourseDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Year { get; set; }
        public string SchoolId { get; set; }
        public string Subject { get; set; }
        public int StudentCount { get; set; }
    }
}