using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.Entities.SchoolAggregate;

namespace SchoolExam.Persistence.Configuration.SchoolAggregate;

public class SchoolTeacherConfiguration : IEntityTypeConfiguration<SchoolTeacher>
{
    public void Configure(EntityTypeBuilder<SchoolTeacher> builder)
    {
        builder.ToTable("SchoolTeacher");
        builder.HasKey(x => new {x.SchoolId, x.TeacherId});
        builder.HasOne<School>().WithMany(School.TeachersName).HasForeignKey(x => x.SchoolId);
        builder.HasOne<Teacher>().WithMany().HasForeignKey(x => x.TeacherId);
    }
}