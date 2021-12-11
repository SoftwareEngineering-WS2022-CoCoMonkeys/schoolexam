namespace SchoolExam.Core.UserManagement.UserAggregate;

public static class Roles
{
    public const string Administrator = "Administrator";
    public const string Teacher = "Teacher";
    public const string Student = "Student";
    public const string LegalGuardian = "LegalGuardian";

    private static readonly List<string> _allRoles = new() {Administrator, Teacher, Student, LegalGuardian};

    public static IEnumerable<string> AllRoles => _allRoles;
}