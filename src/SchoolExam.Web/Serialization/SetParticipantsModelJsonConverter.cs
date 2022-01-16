using SchoolExam.Web.Models.Exam;

namespace SchoolExam.Web.Serialization;

public class SetParticipantsModelJsonConverter : DerivedTypeJsonConverter<ExamParticipantWriteModel>
{
    protected override string TypeToName(Type type)
    {
        if (type == typeof(ExamCourseWriteModel)) return "Course";
        if (type == typeof(ExamStudentWriteModel)) return "Student";

        throw new ArgumentException($"Type {type.FullName} not supported", nameof(type));
    }

    protected override Type NameToType(string typeName)
    {
        return typeName switch
        {
            "Course" => typeof(ExamCourseWriteModel),
            "Student" => typeof(ExamStudentWriteModel),
            _ => throw new ArgumentException($"Unsupported type string \"{typeName}\".", nameof(typeName))
        };
    }
}