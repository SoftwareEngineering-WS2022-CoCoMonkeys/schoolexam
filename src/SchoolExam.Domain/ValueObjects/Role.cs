namespace SchoolExam.Domain.ValueObjects;

public class Role
{
    public const string AdministratorName = "Administrator";
    public static Role Administrator => new(AdministratorName);

    public const string TeacherName = "Teacher";
    public static Role Teacher => new(TeacherName);

    public const string StudentName = "Student";
    public static Role Student => new(StudentName);

    public const string LegalGuardianName = "LegalGuardian";
    public static Role LegalGuardian => new(LegalGuardianName);

    public const string SecretaryName = "Secretary";
    public static Role Secretary => new(SecretaryName);

    public static IEnumerable<Role> AllRoles { get; } =
        new[] {Administrator, Teacher, Student, LegalGuardian, Secretary};

    public string Name { get; }
    
    private Role(string name)
    {
        Name = name;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;
        if (obj is not Role other)
            return false;
        return Name.Equals(other.Name);
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }

    public static implicit operator string(Role role) => role.Name;
}