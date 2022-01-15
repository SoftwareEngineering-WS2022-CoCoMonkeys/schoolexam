using SchoolExam.Domain.Base;

namespace SchoolExam.IntegrationTests.Util;

public interface ITestEntityFactory<out TEntity> where TEntity : IEntity
{
    TEntity Create();
}

public interface ISchoolExamTestEntityFactory
{
    TEntity Create<TEntity>() where TEntity : IEntity;
}