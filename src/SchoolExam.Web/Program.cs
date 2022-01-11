using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using SchoolExam.Application.Authentication;
using SchoolExam.Application.Pdf;
using SchoolExam.Application.QrCode;
using SchoolExam.Application.RandomGenerator;
using SchoolExam.Application.Repository;
using SchoolExam.Application.Services;
using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.Entities.ExamAggregate;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Infrastructure.Authentication;
using SchoolExam.Infrastructure.Pdf;
using SchoolExam.Infrastructure.QrCode;
using SchoolExam.Infrastructure.RandomGenerator;
using SchoolExam.Infrastructure.Repository;
using SchoolExam.Infrastructure.Services;
using SchoolExam.Persistence.Base;
using SchoolExam.Persistence.DataContext;
using SchoolExam.Web.Authorization;
using SchoolExam.Web.Extensions;
using SchoolExam.Web.Mapping;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
    options.AddPolicy(PolicyNames.SubmissionExamCreatorPolicyName, policy =>
    {
        policy.RequireRole(Role.Teacher);
        policy.AddRequirement<SubmissionExamCreatorAuthorizationRequirement>();
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

builder.Services.AddSingleton<IDbConnectionConfiguration>(new DbConnectionConfiguration(connectionString));
builder.Services.AddSingleton<ITokenGenerator, JwtTokenGenerator>();
builder.Services.AddSingleton<IQrCodeGenerator, QRCoderQrCodeGenerator>();
builder.Services.AddSingleton<Random>();
builder.Services.AddSingleton<IRandomGenerator, RandomGenerator>();
builder.Services.AddSingleton<IPdfService, iText7PdfService>();
builder.Services.AddSingleton<IQrCodeReader, ZXingNetQrCodeReader>();
builder.Services.AddTransient<ISchoolExamRepository, SchoolExamRepository>();
builder.Services.AddTransient<ICourseService, CourseService>();
builder.Services.AddTransient<IExamService, ExamService>();
builder.Services.AddTransient<ISubmissionService, SubmissionService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddScoped<ISchoolExamRepositoryInitService, SchoolExamRepositoryInitService>();

var app = builder.Build();

using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var initService = serviceScope.ServiceProvider.GetService<ISchoolExamRepositoryInitService>();
    await initService!.Init();
}

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

app.MapControllers();

app.Run();

public partial class Program
{
}