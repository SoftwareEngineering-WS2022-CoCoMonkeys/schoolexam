using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.PersonAggregate;

namespace SchoolExam.Infrastructure.Specifications;

public class StudentByQrCodeSpecification : EntitySpecification<Student>
{
    public StudentByQrCodeSpecification(string qrCodeData) : base(x => x.QrCode.Data.Equals(qrCodeData))
    {
    }
}