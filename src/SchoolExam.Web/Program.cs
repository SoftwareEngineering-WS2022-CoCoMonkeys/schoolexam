using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using SchoolExam.Application.Authentication;
using SchoolExam.Application.Pdf;
using SchoolExam.Application.Publishing;
using SchoolExam.Application.QrCode;
using SchoolExam.Application.RandomGenerator;
using SchoolExam.Application.Repository;
using SchoolExam.Application.Services;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Infrastructure.Authentication;
using SchoolExam.Infrastructure.Pdf;
using SchoolExam.Infrastructure.Publishing;
using SchoolExam.Infrastructure.QrCode;
using SchoolExam.Infrastructure.RandomGenerator;
using SchoolExam.Infrastructure.Repository;
using SchoolExam.Infrastructure.Services;
using SchoolExam.Persistence.Base;
using SchoolExam.Persistence.DataContext;
using SchoolExam.Web.Authorization;
using SchoolExam.Web.ErrorHandling;
using SchoolExam.Web.Extensions;
using SchoolExam.Web.Mapping;
using SchoolExam.Web.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new ExamParticipantReadModelJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new GradingTableLowerBoundModelJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new ExamParticipantWriteModelJsonConverter());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => { options.UseAllOfForInheritance(); });
builder.Services.AddAutoMapper(config => { config.AddProfile<SchoolExamMappingProfile>(); });

var key = Base64UrlEncoder.DecodeBytes("gLGtlGNQw8n7iHxUFjuDmHFcPRDUteRROdqhbhCstxEOIiit6kBT6exFo0Lm5uR");
SymmetricSecurityKey signingKey = new SymmetricSecurityKey(key);

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
        policy.AddRequirement<CourseTeacherAuthorizationRequirement>();
    });
    options.AddPolicy(PolicyNames.CourseStudentPolicyName, policy =>
    {
        policy.RequireRole(Role.Student);
        policy.AddRequirement<CourseStudentAuthorizationRequirement>();
    });

    // ExamController authorization policies
    options.AddPolicy(PolicyNames.ExamCreatorPolicyName, policy =>
    {
        policy.RequireRole(Role.Teacher);
        policy.AddRequirement<ExamCreatorAuthorizationRequirement>();
    });

    // SubmissionController authorization policies
    options.AddPolicy(PolicyNames.SubmissionExamCreatorPolicyName, policy =>
    {
        policy.RequireRole(Role.Teacher);
        policy.AddRequirement<SubmissionExamCreatorAuthorizationRequirement>();
    });
    options.AddPolicy(PolicyNames.SubmissionsExamCreatorPolicyName, policy =>
    {
        policy.RequireRole(Role.Teacher);
        policy.AddRequirement<SubmissionsExamCreatorAuthorizationRequirement>();
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.Configure<FormOptions>(options =>
{
    // maximum file size limit of 50MB
    options.MultipartBodyLengthLimit = 52428800;
});

builder.Services.AddScoped<IAuthorizationHandler, CourseTeacherAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CourseStudentAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ExamCreatorAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, SubmissionExamCreatorAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, SubmissionsExamCreatorAuthorizationHandler>();

builder.Services.AddDbContext<SchoolExamDbContext>();
builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

var connectionString = "";
var databaseUrl = builder.Configuration["DATABASE_URL"];
if (databaseUrl != null)
{
    var databaseUri = new Uri(databaseUrl);
    var userInfo = databaseUri.UserInfo.Split(':');
    connectionString =
        $"host={databaseUri.Host};port={databaseUri.Port};username={userInfo[0]};password={userInfo[1]};database={databaseUri.LocalPath.TrimStart('/')};pooling=true;";
}

var resetDatabase = builder.Configuration["RESET_DATABASE"]?.Equals("1");

builder.Services.AddSingleton<IDbConnectionConfiguration>(new DbConnectionConfiguration(connectionString));
builder.Services.AddSingleton<ITokenGenerator, JwtTokenGenerator>();
builder.Services.AddSingleton<IQrCodeGenerator, QRCoderQrCodeGenerator>();
builder.Services.AddSingleton<Random>();
builder.Services.AddSingleton<IRandomGenerator, RandomGenerator>();
builder.Services.AddSingleton<IPdfService, iText7PdfService>();
builder.Services.AddSingleton<IQrCodeReader, ZXingNetQrCodeReader>();
builder.Services.AddTransient<IPublishingService, PublishingService>();
builder.Services.AddTransient<ISchoolExamRepository, SchoolExamRepository>();
builder.Services.AddTransient<ICourseService, CourseService>();
builder.Services.AddTransient<IExamManagementService, ExamManagementService>();
builder.Services.AddTransient<IExamTaskService, ExamTaskService>();
builder.Services.AddTransient<IExamBuildService, ExamBuildService>();
builder.Services.AddTransient<IMatchingService, MatchingService>();
builder.Services.AddTransient<ICorrectionService, CorrectionService>();
builder.Services.AddTransient<ISubmissionService, SubmissionService>();
builder.Services.AddTransient<IExamPublishService, ExamPublishService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IPersonService, PersonService>();

builder.Services.AddScoped<ISchoolExamRepositoryInitService, SchoolExamRepositoryInitService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// swagger also added for production to make development easier
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("CorsPolicy");

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapControllers();

using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var initService = serviceScope.ServiceProvider.GetService<ISchoolExamRepositoryInitService>();
    await initService!.Init(resetDatabase ?? false);   
}

app.Run();

public partial class Program
{
}