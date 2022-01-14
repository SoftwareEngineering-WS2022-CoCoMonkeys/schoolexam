using System;
using System.Linq;
using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.ExamAggregate;

namespace SchoolExam.IntegrationTests.Util.Specifications;

public class ExamByCourseSpecification : EntitySpecification<Exam>
{
    public ExamByCourseSpecification(Guid courseId) : base(x =>
        x.Participants.Where(p => p is ExamCourse).Any(p => p.ParticipantId.Equals(courseId)))
    {
        AddInclude(x => x.Participants);
    }
}