using System;
using SchoolExam.Persistence.Base;

namespace SchoolExam.IntegrationTests.Util.Mock;

public class EntityFrameworkInMemoryDbConnectionConfiguration : IDbConnectionConfiguration
{
    private Guid _databaseName = Guid.NewGuid();
    public string ConnectionString => _databaseName.ToString();
    public DbTechnology Technology => DbTechnology.EntityFrameworkInMemory;

    public void NewDbName()
    {
        _databaseName = Guid.NewGuid();
    }
}