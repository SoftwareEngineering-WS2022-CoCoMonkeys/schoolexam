using SchoolExam.Domain.Base;

namespace SchoolExam.IntegrationTests.Util;

public interface ITestEntityFactory<out TEntity, in TIdentity> where TEntity : IEntity<TIdentity>
{
    TEntity Create();
}

public interface ISchoolExamTestEntityFactory
{
    TEntity Create<TEntity, TIdentity>() where TEntity : IEntity<TIdentity>;
}