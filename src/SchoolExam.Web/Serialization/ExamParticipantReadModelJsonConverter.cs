using SchoolExam.Web.Models.Exam;

namespace SchoolExam.Web.Serialization;

public class ExamParticipantReadModelJsonConverter : DerivedTypeJsonConverter<ExamParticipantReadModel>
{
    protected override string TypeToName(Type type)
    {
        if (type == typeof(ExamCourseReadModel)) return "Course";
        if (type == typeof(ExamStudentReadModel)) return "Student";

        throw new ArgumentException($"Type {type.FullName} not supported", nameof(type));
    }

    protected override Type NameToType(string typeName)
    {
        return typeName switch
        {
            "Course" => typeof(ExamCourseReadModel),
            "Student" => typeof(ExamStudentReadModel),
            _ => throw new ArgumentException($"Unsupported type string \"{typeName}\".", nameof(typeName))
        };
    }
}