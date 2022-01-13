using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Persistence.Extensions;

namespace SchoolExam.Persistence.Configuration.CourseAggregate;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Course");
        builder.HasKey(x => x.Id);
        builder.OwnsTopic(x => x.Topic, false, new
        {
            CourseId = SeedIds.SozialwissenschaftenCourseId,
            Name = "Sozialwissenschaften"
        });
        builder.HasData(new
        {
            Id = SeedIds.SozialwissenschaftenCourseId, Name = "Sozialwissenschaften 2022",
            Description = "Projektmanagement, etc.", SchoolId = SeedIds.GymnasiumDiedorfId
        });
    }
}