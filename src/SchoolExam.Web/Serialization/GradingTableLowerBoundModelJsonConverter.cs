using SchoolExam.Web.Models.Exam;

namespace SchoolExam.Web.Serialization;

public class GradingTableLowerBoundModelJsonConverter : DerivedTypeJsonConverter<GradingTableLowerBoundModelBase>
{
    protected override string TypeToName(Type type)
    {
        if (type == typeof(GradingTableLowerBoundPointsModel)) return "Points";
        if (type == typeof(GradingTableLowerBoundPercentageModel)) return "Percentage";

        throw new ArgumentException($"Type {type.FullName} not supported", nameof(type));
    }

    protected override Type NameToType(string typeName)
    {
        return typeName switch
        {
            "Points" => typeof(GradingTableLowerBoundPointsModel),
            "Percentage" => typeof(GradingTableLowerBoundPercentageModel),
            _ => throw new ArgumentException($"Unsupported type string \"{typeName}\".", nameof(typeName))
        };
    }
}