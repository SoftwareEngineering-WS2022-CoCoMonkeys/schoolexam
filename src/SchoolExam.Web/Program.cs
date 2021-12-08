using Common.Infrastructure.EFAbstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SchoolExam.Core.Domain.CourseAggregate;
using SchoolExam.Infrastructure.DataContext;
using SchoolExam.Infrastructure.Repository;
using SchoolExam.Web.Mapping;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(SchoolExamMappingProfile));

builder.Services.AddDbContext<SchoolExamDbContext>();
builder.Services.AddSingleton<IDbConnectionConfiguration, DbConnectionConfiguration>();
builder.Services.AddTransient<SchoolExamDataContext>();
builder.Services.AddTransient<ICourseRepository, CourseRepository>();

var app = builder.Build();

using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetService<SchoolExamDbContext>();
    context!.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();