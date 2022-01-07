namespace SchoolExam.Domain.Entities.PersonAggregate;

public class StudentLegalGuardian
{
    public Guid StudentId { get; set; }
    public Guid LegalGuardianId { get; set; }

    public StudentLegalGuardian(Guid studentId, Guid legalGuardianId)
    {
        StudentId = studentId;
        LegalGuardianId = legalGuardianId;
    }
}