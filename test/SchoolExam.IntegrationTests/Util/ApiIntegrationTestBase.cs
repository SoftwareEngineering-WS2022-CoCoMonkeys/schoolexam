using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SchoolExam.Application.Authentication;
using SchoolExam.Application.Repository;
using SchoolExam.Infrastructure.Repository;
using SchoolExam.IntegrationTests.Util.Mock;
using SchoolExam.Persistence.Base;
using SchoolExam.Persistence.DataContext;

namespace SchoolExam.IntegrationTests.Util;

public abstract class ApiIntegrationTestBase
{
    protected HttpClient Client { get; private set; } = null!;
    protected ISchoolExamTestEntityFactory TestEntityFactory { get; }

    private readonly WebApplicationFactory<Program> _factory;

    protected ApiIntegrationTestBase()
    {
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
                {
                    services
                        .AddSingleton<IDbConnectionConfiguration,
                            EntityFrameworkInMemoryDbConnectionConfiguration>();
                    services.AddSingleton<IPasswordHasher, TestPasswordHasher>();
                    services.AddScoped<ISchoolExamRepositoryInitService, TestSchoolExamRepositoryInitService>();
                    services.AddAuthentication(options =>
                        {
                            options.DefaultAuthenticateScheme = TestAuthenticationHandler.AuthenticationScheme;
                            options.DefaultScheme = TestAuthenticationHandler.AuthenticationScheme;
                        })
                        .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                            TestAuthenticationHandler.AuthenticationScheme, _ => { });
                })
                .ConfigureTestServices(ConfigureTestServices);
        });
        TestEntityFactory = new AutoFixtureTestEntityFactory();
    }

    protected virtual void ConfigureTestServices(IServiceCollection services)
    {
    }

    protected T GetRequiredService<T>() where T : notnull
    {
        return _factory.Services.GetRequiredService<T>();
    }

    [OneTimeSetUp]
    protected virtual void OneTimeSetUp()
    {
        Client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(TestAuthenticationHandler.AuthenticationScheme);
    }

    [SetUp]
    protected virtual void SetUp()
    {
        var configuration =
            GetRequiredService<IDbConnectionConfiguration>() as EntityFrameworkInMemoryDbConnectionConfiguration;
        configuration?.NewDbName();
        SetUpData();
    }

    protected virtual void SetUpData()
    {
    }

    [TearDown]
    protected virtual void TearDown()
    {
    }

    [OneTimeTearDown]
    protected virtual void OneTimeTearDown()
    {
        Client.Dispose();
    }

    private SchoolExamDbContext GetSchoolExamDbContext()
    {
        var configuration = GetRequiredService<IDbConnectionConfiguration>();
        var context = new SchoolExamDbContext(configuration);

        return context;
    }

    protected ISchoolExamRepository GetSchoolExamRepository()
    {
        var context = GetSchoolExamDbContext();
        var repository = new SchoolExamRepository(context);

        return repository;
    }

    protected void SetClaims(params Claim[] claims)
    {
        TestAuthenticationHandler.SetClaims(claims);
    }
}