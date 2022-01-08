using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using SchoolExam.Application.Authentication;
using SchoolExam.Application.DataContext;
using SchoolExam.Application.Pdf;
using SchoolExam.Application.QrCode;
using SchoolExam.Application.RandomGenerator;
using SchoolExam.Application.Repositories;
using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Infrastructure.Authentication;
using SchoolExam.Infrastructure.DataContext;
using SchoolExam.Infrastructure.Pdf;
using SchoolExam.Infrastructure.QrCode;
using SchoolExam.Infrastructure.RandomGenerator;
using SchoolExam.Infrastructure.Repositories;
using SchoolExam.Persistence.Base;
using SchoolExam.Persistence.DataContext;
using SchoolExam.Web.Authorization;
using SchoolExam.Web.Mapping;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<SchoolExamMappingProfile>();
});

var key = Base64UrlEncoder.DecodeBytes("gLGtlGNQw8n7iHxUFjuDmHFcPRDUteRROdqhbhCstxEOIiit6kBT6exFo0Lm5uR");
SymmetricSecurityKey signingKey = new SymmetricSecurityKey(key);

var port = builder.Configuration["PORT"];
builder.WebHost.UseUrls("http://*:" + port);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false,
        ValidateIssuer = false,
        IssuerSigningKey = signingKey,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddAuthorization(options =>
{
    // CourseController authorization policies
    options.AddPolicy(PolicyNames.CourseTeacherPolicyName, policy =>
    {
        policy.RequireRole(Role.Teacher);
        policy.AddRequirements(new OwnerRequirement<Course>(course => course.Teachers.Select(x => x.TeacherId),
            RouteParameterNames.CourseIdParameterName, nameof(Course.Teachers)));
    });
    options.AddPolicy(PolicyNames.CourseStudentPolicyName, policy =>
    {
        policy.RequireRole(Role.Student);
        policy.AddRequirements(new OwnerRequirement<Course>(course => course.Students.Select(x => x.StudentId),
            RouteParameterNames.CourseIdParameterName, nameof(Course.Students)));
    });
    
    // ExamController authorization policies
    options.AddPolicy(PolicyNames.ExamCreatorPolicyName, policy =>
    {
        policy.RequireRole(Role.Teacher);
        policy.AddRequirements(new OwnerRequirement<Exam>(exam => exam.CreatorId,
            RouteParameterNames.ExamIdParameterName));
    });
});

builder.Services.Configure<FormOptions>(options =>
{
    // maximum file size limit of 50MB
    options.MultipartBodyLengthLimit = 52428800;
});

builder.Services.AddScoped<IAuthorizationHandler, OwnerHandler<Course>>();
builder.Services.AddScoped<IAuthorizationHandler, OwnerHandler<Exam>>();

builder.Services.AddDbContext<SchoolExamDbContext>();
builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

builder.Configuration.AddEnvironmentVariables(prefix: "SCHOOLEXAM_");

var databaseUrl = builder.Configuration["DATABASE_URL"];
var databaseUri = new Uri(databaseUrl);
var userInfo = databaseUri.UserInfo.Split(':');
var connectionString =
    $"host={databaseUri.Host};port={databaseUri.Port};username={userInfo[0]};password={userInfo[1]};database={databaseUri.LocalPath.TrimStart('/')};pooling=true;";

builder.Services.AddSingleton<IDbConnectionConfiguration>(new DbConnectionConfiguration(connectionString));
builder.Services.AddSingleton<ITokenGenerator, JwtTokenGenerator>();
builder.Services.AddSingleton<IQrCodeGenerator, QRCoderQrCodeGenerator>();
builder.Services.AddSingleton<Random>();
builder.Services.AddSingleton<IRandomGenerator, RandomGenerator>();
builder.Services.AddSingleton<IPdfService, iText7PdfService>();
builder.Services.AddSingleton<IQrCodeReader, ZXingNetQrCodeReader>();
builder.Services.AddTransient<ISchoolExamDataContext, SchoolExamDataContext>();
builder.Services.AddTransient<ICourseRepository, CourseRepository>();
builder.Services.AddTransient<IExamRepository, ExamRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISchoolExamDataContextInitService, SchoolExamDataContextInitService>();

var app = builder.Build();

using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var initService = serviceScope.ServiceProvider.GetService<ISchoolExamDataContextInitService>();
    await initService!.Init();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{
}