using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using SchoolExam.Application.Authentication;
using SchoolExam.Application.DataContext;
using SchoolExam.Application.QrCode;
using SchoolExam.Application.RandomGenerator;
using SchoolExam.Application.Repositories;
using SchoolExam.Domain.Entities.CourseAggregate;
using SchoolExam.Domain.ValueObjects;
using SchoolExam.Infrastructure.Authentication;
using SchoolExam.Infrastructure.DataContext;
using SchoolExam.Infrastructure.QrCode;
using SchoolExam.Infrastructure.RandomGenerator;
using SchoolExam.Infrastructure.Repositories;
using SchoolExam.Persistence.Base;
using SchoolExam.Persistence.DataContext;
using SchoolExam.Web.Authorization;
using SchoolExam.Web.Course;
using SchoolExam.Web.Mapping;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(SchoolExamMappingProfile));

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(CourseController.CourseTeacherPolicyName, policy =>
    {
        policy.RequireRole(Role.Teacher);
        policy.AddRequirements(new OwnerRequirement<Course>(course => course.TeacherIds,
            CourseController.CourseIdParameterName, Course.TeachersName));
    });
    options.AddPolicy(CourseController.CourseStudentPolicyName, policy =>
    {
        policy.RequireRole(Role.Student);
        policy.AddRequirements(new OwnerRequirement<Course>(course => course.StudentIds,
            CourseController.CourseIdParameterName, Course.StudentsName));
    });
});

builder.Services.AddScoped<IAuthorizationHandler, OwnerHandler<Course>>();

builder.Services.AddDbContext<SchoolExamDbContext>();
builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddSingleton<IDbConnectionConfiguration, DbConnectionConfiguration>();
builder.Services.AddSingleton<ITokenGenerator, JwtTokenGenerator>();
builder.Services.AddSingleton<IQrCodeGenerator, QRCoderQrCodeGenerator>();
builder.Services.AddSingleton<Random>();
builder.Services.AddSingleton<IRandomGenerator, RandomGenerator>();
builder.Services.AddTransient<ISchoolExamDataContext, SchoolExamDataContext>();
builder.Services.AddTransient<ICourseRepository, CourseRepository>();
builder.Services.AddTransient<IExamRepository, ExamRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();

var app = builder.Build();

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