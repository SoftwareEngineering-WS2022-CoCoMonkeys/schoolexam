using System.Net;
using System.Net.Mime;
using System.Text.Json;
using SchoolExam.Domain.Exceptions;
using SchoolExam.Web.Models.ErrorHandling;

namespace SchoolExam.Web.ErrorHandling;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            var response = context.Response;
            response.ContentType = MediaTypeNames.Application.Json;
            
            switch (exception)
            {
                case DomainException:
                    response.StatusCode = (int) HttpStatusCode.BadRequest;
                    break;
                default:
                    response.StatusCode = (int) HttpStatusCode.InternalServerError;
                    break;
            }

            var result = JsonSerializer.Serialize(new ErrorReadModel {Message = exception.Message});
            await response.WriteAsync(result);
        }
    }
}