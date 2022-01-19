namespace SchoolExam.Web.Models.Exam;

public class ExamCourseReadModel : ExamParticipantReadModel
{
    public IEnumerable<ExamStudentReadModel> Children { get; set; } = null!;
}