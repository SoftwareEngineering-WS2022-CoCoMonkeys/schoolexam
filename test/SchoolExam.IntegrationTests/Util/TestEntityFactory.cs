using System;
using AutoFixture;
using SchoolExam.Domain.Base;
using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.Entities.PersonAggregate;
using SchoolExam.Domain.Entities.SchoolAggregate;

namespace SchoolExam.IntegrationTests.Util;

public class AutoFixtureTestEntityFactory : ISchoolExamTestEntityFactory,
    ITestEntityFactory<School, Guid>,
    ITestEntityFactory<Course, Guid>,
    ITestEntityFactory<Teacher, Guid>,
    ITestEntityFactory<Student, Guid>
{
    private readonly Fixture _fixture;

    public AutoFixtureTestEntityFactory()
    {
        _fixture = new Fixture();
    }

    public TEntity Create<TEntity, TIdentity>() where TEntity : IEntity<TIdentity>
    {
        ITestEntityFactory<TEntity, TIdentity>? factory = this as ITestEntityFactory<TEntity, TIdentity>;
        if (factory == null)
        {
            throw new ArgumentException($"No factory method implemented for entity type {typeof(TEntity).Name}.");
        }

        return factory.Create();
    }

    School ITestEntityFactory<School, Guid>.Create()
    {
        return _fixture.Create<School>();
    }

    Course ITestEntityFactory<Course, Guid>.Create()
    {
        return _fixture.Create<Course>();
    }

    Teacher ITestEntityFactory<Teacher, Guid>.Create()
    {
        return _fixture.Create<Teacher>();
    }

    Student ITestEntityFactory<Student, Guid>.Create()
    {
        return _fixture.Create<Student>();
    }
}